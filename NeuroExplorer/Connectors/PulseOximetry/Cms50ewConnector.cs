using System;
using System.IO;
using System.Text;
using System.Threading;

namespace NeuroExplorer.Connectors.PulseOximetry
{
    class Cms50ewConnector
    {
        public bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        private string ByteToBitsString(byte byteIn)
        {
            var bitsString = new StringBuilder(8);

            bitsString.Append(Convert.ToString((byteIn / 128) % 2));
            bitsString.Append(Convert.ToString((byteIn / 64) % 2));
            bitsString.Append(Convert.ToString((byteIn / 32) % 2));
            bitsString.Append(Convert.ToString((byteIn / 16) % 2));
            bitsString.Append(Convert.ToString((byteIn / 8) % 2));
            bitsString.Append(Convert.ToString((byteIn / 4) % 2));
            bitsString.Append(Convert.ToString((byteIn / 2) % 2));
            bitsString.Append(Convert.ToString((byteIn / 1) % 2));
            return bitsString.ToString();
        }

        private void ParseStream(Stream stream)
        {

            int IOExceptionsCount = 0;
            int IOExceptionsLimit = 10;

            stream.ReadTimeout = 100;

            while (IOExceptionsCount < IOExceptionsLimit)
            {
                byte[] data = new byte[10];
                try
                {
                    stream.Write(new byte[] { 0, 0x7D, 0x81, 0xA1, 0x80, 0x80, 0x80, 0x80 }, 0, 8);
                    Thread.Sleep(16); // 60 samples per second
                    stream.Read(data, 0, data.Length);
                    stream.Flush();
                }
                catch
                {
                    IOExceptionsCount++;
                    continue;
                }
                int checker = data[0];
                if (checker != 1)
                {
                    continue;
                }
                int signal_strength = data[2] & 0xf;
                int pulse_waveform = data[3] & 0x7f;
                int bar_graph = data[4] & 0xf;
                int pulse = data[5] & 0x7f;
                int spo2 = data[6] & 0x7f;
                if (signal_strength > 8)
                {
                    signal_strength = 8;
                }

                if ((spo2 == 0) || (pulse == 0))
                {
                    //UpdateUI("waiting for data...");
                    continue;
                }
                else
                {
                    double signal_strength_percent = signal_strength * 12.5;
                    signal_strength_percent = System.Math.Round(signal_strength_percent, 0);
                    //UpdateRates(signal_strength, pulse_waveform, bar_graph, pulse, spo2);
                    //UpdateUI(" OK (" + signal_strength_percent + "%)");
                }
            }

            stream.Write(new byte[] { 0, 0x7D, 0x81, 0xA2, 0x80, 0x80, 0x80, 0x80 }, 0, 8);
        }
    }
}
