using Fleck;
using NeuroExplorer.LogWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NeuroExplorer.WebSocket
{
    class WebSocketConnector
    {

        public Action<string> onMessage;

        private WebSocketServer webSocket;
        private List<KeyValuePair<string, List<IWebSocketConnection>>> allSockets = new List<KeyValuePair<string, List<IWebSocketConnection>>>();
        private List<KeyValuePair<string, List<string>>> onConnectMessages = new List<KeyValuePair<string, List<string>>>();
        
        public WebSocketConnector()
        {
            onMessage += message => Propagate("/cmd", message);
        }

        public void RegisterEndpoint(string endpoint, List<string> greetingsMessages)
        {
            lock (allSockets)
            {
                if (!allSockets.Where(kvp => kvp.Key == endpoint).Any())
                {
                    allSockets.Add(new KeyValuePair<string, List<IWebSocketConnection>>(endpoint, new List<IWebSocketConnection>()));
                    onConnectMessages.Add(new KeyValuePair<string, List<string>>(endpoint, greetingsMessages));
                }
            }
        }

        public void Connect(string address)
        {
            webSocket = new WebSocketServer(address);
            webSocket.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    lock (allSockets)
                    {
                        if (allSockets.Where(kvp => kvp.Key == socket.ConnectionInfo.Path).Any())
                        {
                            allSockets.Find(kvp => kvp.Key == socket.ConnectionInfo.Path).Value.Add(socket);
                        }
                        if (onConnectMessages.Where(kvp => kvp.Key == socket.ConnectionInfo.Path).Any())
                        {
                            foreach (string message in onConnectMessages.Find(kvp => kvp.Key == socket.ConnectionInfo.Path).Value)
                            {
                                socket.Send(message);
                            }
                        }
                    }
                };
                socket.OnClose = () =>
                {
                    lock (allSockets)
                    {
                        foreach (KeyValuePair<string, List<IWebSocketConnection>> kvp in allSockets)
                        {
                            kvp.Value.RemoveAll(ws => ws == socket);
                        }
                    }
                };
                socket.OnError = e =>
                {
                    lock (allSockets)
                    {
                        foreach (KeyValuePair<string, List<IWebSocketConnection>> kvp in allSockets)
                        {
                            kvp.Value.RemoveAll(ws => ws == socket);
                        }
                    }
                };
                socket.OnMessage = message =>
                {
                    onMessage(message);
                };
            });
        }

        public void Disconnect()
        {
            allSockets.Clear();
            if (webSocket == null)
            {
                return;
            }
            webSocket.Dispose();
        }

        public void Propagate(string endpoint, string message)
        {
            Propagate(endpoint, 0, null, message);
        }

        public void Propagate(string endpoint, byte[] message)
        {
            Propagate(endpoint, 1, message, null);
        }

        public void Propagate(string endpoint, int type, byte[] bmessage = null, string smessage = null)
        {
            if (webSocket == null)
            {
                return;
            }
            List<KeyValuePair<string, List<IWebSocketConnection>>> currentSockets;
            lock (allSockets)
            {
                currentSockets = allSockets.FindAll(kvp => kvp.Key == endpoint).ToList();
            }
            foreach (KeyValuePair<string, List<IWebSocketConnection>> kvp in currentSockets)
            {
                List<IWebSocketConnection> currentKvp = kvp.Value.ToList();
                foreach (IWebSocketConnection socket in currentKvp)
                {
                    if(socket == null ||!socket.IsAvailable)
                    {
                        continue;
                    }
                    switch (type)
                    {
                        case 0:
                            socket.Send(smessage);
                            break;
                        case 1:
                            socket.Send(bmessage);
                            break;
                    }
                }
            }
        }
    }
}
