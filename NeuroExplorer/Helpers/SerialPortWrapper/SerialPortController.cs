using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NeuroExplorer.Helpers.SerialPortWrapper
{
    class SerialPortController : IDisposable
    {

        private SerialPort _serialPort;
        private SerialSettings _currentSerialSettings = new SerialSettings();
        private string _latestRecieved = String.Empty;
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

        public SerialPortController()
        {
            _currentSerialSettings.PortNameCollection = SerialPort.GetPortNames();
            _currentSerialSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_currentSerialSettings_PropertyChanged);

            if (_currentSerialSettings.PortNameCollection.Length > 0)
            {
                _currentSerialSettings.PortName = _currentSerialSettings.PortNameCollection[0];
            }
        }

        public List<string> GetActiveComPorts()
        {
            List<string> output = new List<string>();
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                output.Add(ports[i]);
            }
            return output;
        }

        public static List<KeyValuePair<string, string>> GetComPortsByVID(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<KeyValuePair<string, string>> comports = new List<KeyValuePair<string, string>>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add(new KeyValuePair<string, string>((string)rk5.Name, (string)rk6.GetValue("PortName")));
                        }
                    }
                }
            }
            return comports;
        }

        ~SerialPortController()
        {
            Dispose(false);
        }

        public SerialSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set { _currentSerialSettings = value; }
        }


        void _currentSerialSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PortName"))
            {
                //UpdateBaudRateCollection();
            }
        }


        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int dataLength = _serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = _serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
            {
                return;
            }

            if (NewSerialDataRecieved != null)
            {
                NewSerialDataRecieved(this, new SerialDataEventArgs(data));
            }
        }
    

        public void StartListening()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort = new SerialPort(
                    _currentSerialSettings.PortName,
                    _currentSerialSettings.BaudRate,
                    _currentSerialSettings.Parity,
                    _currentSerialSettings.DataBits,
                    _currentSerialSettings.StopBits);

            _serialPort.DtrEnable = _currentSerialSettings.DtrEnable;
            _serialPort.RtsEnable = _currentSerialSettings.RtsEnable;
            _serialPort.ReceivedBytesThreshold = _currentSerialSettings.ReceivedBytesThreshold;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            _serialPort.Open();
        }

        public void StopListening()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        private void UpdateBaudRateCollection()
        {
            _serialPort = new SerialPort(_currentSerialSettings.PortName);
            _serialPort.Open();
            object p = _serialPort.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_serialPort.BaseStream);
            Int32 dwSettableBaud = (Int32)p.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);

            _serialPort.Close();
            _currentSerialSettings.UpdateBaudRateCollection(dwSettableBaud);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            }
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                _serialPort.Dispose();
            }
        }

        public static List<KeyValuePair<string, string>> GetBluetoothPorts()
        {
            Regex regexPortName = new Regex(@"(COM\d+)");

            List<KeyValuePair<string, string>> comports = new List<KeyValuePair<string, string>>();

            ManagementObjectSearcher searchSerial = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

            foreach (ManagementObject obj in searchSerial.Get())
            {
                string name = obj["Name"] as string;
                string classGuid = obj["ClassGuid"] as string;
                string deviceID = obj["DeviceID"] as string;

                if (classGuid != null && deviceID != null)
                {
                    if (String.Equals(classGuid, "{4d36e978-e325-11ce-bfc1-08002be10318}", StringComparison.InvariantCulture))
                    {
                        string[] tokens = deviceID.Split('&');

                        if (tokens.Length >= 4)
                        {
                            string[] addressToken = tokens[4].Split('_');
                            string bluetoothAddress = addressToken[0];

                            Match m = regexPortName.Match(name);
                            string comPortNumber = "";
                            if (m.Success)
                            {
                                comPortNumber = m.Groups[1].ToString();
                            }

                            if (Convert.ToUInt64(bluetoothAddress, 16) > 0)
                            {
                                string bluetoothName = GetBluetoothRegistryName(bluetoothAddress);
                                comports.Add(new KeyValuePair<string, string>(bluetoothName, comPortNumber));
                            }
                        }
                    }
                }
            }

            return comports;
        }

        private static string GetBluetoothRegistryName(string address)
        {
            string deviceName = "";

            string registryPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";
            string devicePath = String.Format(@"{0}\{1}", registryPath, address);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(devicePath))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Name");

                    byte[] raw = o as byte[];

                    if (raw != null)
                    {
                        deviceName = Encoding.ASCII.GetString(raw);
                    }
                }
            }

            return deviceName;
        }

    }
}
