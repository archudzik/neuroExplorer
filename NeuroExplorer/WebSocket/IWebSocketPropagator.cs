using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroExplorer.WebSocket
{
    interface IWebSocketPropagator
    {
        void SetWebSocket(WebSocketConnector ws);
        void Init();
        void Connect();
        void Configure();
        void Disconnect();
        void SetStatus(string newStatus);
        string GetStatus();
        void StartRecording(string outputFolder);
        void StopRecording();
        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e);
    }
}
