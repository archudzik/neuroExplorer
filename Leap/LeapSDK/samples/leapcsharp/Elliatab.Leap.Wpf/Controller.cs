using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elliatab.Leap
{
    public sealed class Controller : IDisposable
    {
        private static readonly SynchronizationContext DefaultContext = new SynchronizationContext();
        private readonly Uri defaultUri = new Uri("ws://localhost:6437/");
        private readonly SynchronizationContext synchronizationContext;
        private readonly Action<Frame> handler;
        private readonly SendOrPostCallback invokeHandlers;
        private readonly ClientWebSocket ws = new ClientWebSocket();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private bool disposed;

        public Controller()
        {
            this.synchronizationContext = SynchronizationContext.Current ?? DefaultContext;
            this.invokeHandlers = this.InvokeHandlers;

            ConnectAndReadInput(ws, cts.Token);
        }

        public Controller(Action<Frame> handler)
            : this()
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            this.handler = handler;
        }

        ~Controller()
        {
            this.Dispose(false);
        }

        public event EventHandler<Frame> FrameAcquired;

        /// <summary>
        /// Gets the current version of the API.
        /// </summary>
        public VersionInfo Version
        {
            get;
            private set;
        }

        private void OnNewFrame(Frame frame)
        {
            if (this.handler == null && this.FrameAcquired == null)
            {
                return;
            }

            this.synchronizationContext.Post(this.invokeHandlers, frame);
        }

        private void InvokeHandlers(object state)
        {
            var e = (Frame)state;
            Action<Frame> action = this.handler;
            EventHandler<Frame> eventHandler = this.FrameAcquired;
            if (action != null)
                action(e);
            if (eventHandler == null)
                return;
            eventHandler(this, e);
        }

        private async void ConnectAndReadInput(ClientWebSocket websocket, CancellationToken ct)
        {
            await websocket.ConnectAsync(this.defaultUri, ct);

            Task.Factory.StartNew(this.ReceiveData, ct,
                            TaskCreationOptions.LongRunning);
        }

        private async void ReceiveData(object state)
        {
            var cancellationToken = (CancellationToken)state;

            var firstFrame = true;
            var rcvBytes = new byte[4096];
            var rcvBuffer = new ArraySegment<byte>(rcvBytes);
            int completeMessageSize = 0;
            
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var rcvResult = await ws.ReceiveAsync(rcvBuffer, cancellationToken);

                    completeMessageSize += rcvResult.Count;

                    if (!rcvResult.EndOfMessage)
                    {
                        var previousOffset = rcvBuffer.Offset;
                        int currentOffset = rcvResult.Count + previousOffset;
                        var newCount = rcvBytes.Length - currentOffset;
                        rcvBuffer = new ArraySegment<byte>(rcvBytes, currentOffset, newCount);
                    }
                    else
                    {
                        rcvBuffer = new ArraySegment<byte>(rcvBytes);
                        byte[] msgBytes = rcvBuffer.Take(completeMessageSize).ToArray();

                        string rcvMsg = Encoding.UTF8.GetString(msgBytes);

                        if (firstFrame)
                        {
                            this.Version = VersionInfo.DeserializeFromJson(rcvMsg);
                            firstFrame = false;
                        }
                        else
                        {
                            var frame = Frame.DeserializeFromJson(rcvMsg);
                            if (frame != null)
                            {
                                this.OnNewFrame(frame);
                            }
                        }

                        completeMessageSize = 0;
                    }
                }
            }
            catch (WebSocketException wex)
            {
                var status = wex.WebSocketErrorCode;

                switch (status)
                {
                    case WebSocketError.InvalidState:
                        break; // Nothing to do, the connection was closed by user.
                    case WebSocketError.ConnectionClosedPrematurely:
                        // TODO: disconnected event ?
                        break;
                }
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debug.WriteLine("WebSocket object already disposed.");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Read operation cancelled by user.");
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                cts.Cancel();
                this.ws.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
