using System;

namespace NeuroExplorer.Helpers.SerialPortWrapper
{
    public class SerialDataEventArgs : EventArgs
    {
        public byte[] Data;

        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }

    }
}
