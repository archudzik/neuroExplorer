using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace Harthoorn.MuseClient
{

    public class Advertisement
    {
        public string Name;
        public ulong Address;
        public short SignalStrengh; 
    }

    public class BleScanner
    {
        public event Action<Advertisement> OnAdvertise;
        public bool scanning = false;
        public BluetoothLEAdvertisementWatcher watcher;

        public void ScanStart()
        {
            // Create Bluetooth Listener
            watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Only activate the watcher when we're recieving values >= -80
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // Stop watching if the value drops below -90 (user walked away)
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();

        }

        public async Task EnumerateDevicesAsync()
        {
            //Paired bluetooth devices
            DeviceInformationCollection PairedBluetoothDevices =
                   await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            //Connected bluetooth devices
            DeviceInformationCollection ConnectedBluetoothDevices =
                   await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromConnectionStatus(BluetoothConnectionStatus.Connected));
        }

        public void ScanStop()
        {
            if(watcher != null)
            {
                watcher.Stop();
            }
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            if (eventArgs.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse) return;
            var adv = new Advertisement
            {
                Address = eventArgs.BluetoothAddress,
                Name = eventArgs.Advertisement.LocalName,
                SignalStrengh = eventArgs.RawSignalStrengthInDBm
            };
            this.OnAdvertise?.Invoke(adv);
        }



    }
}

 