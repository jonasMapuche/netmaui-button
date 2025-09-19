using net_maui_app_v24.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using System.Collections.ObjectModel;
using System.Text;

namespace net_maui_app_v24.Services
{
    public class BluetoothService
    {

        private BluetoothState crossBluetoothLE;
        private IAdapter adapterBluetooth;
        public ObservableCollection<DeviceBluetooth> deviceBluetooth;

        public static bool State { get => CrossBluetoothLE.Current.State == BluetoothState.On ? true : false; }

        public BluetoothService()
        {
            this.crossBluetoothLE = CrossBluetoothLE.Current.State;
            this.adapterBluetooth = CrossBluetoothLE.Current.Adapter;
            this.adapterBluetooth.ScanMode = ScanMode.LowLatency;
            this.adapterBluetooth.ScanTimeout = 3000000;
        }

        public static string Message()
        {
            var result = "Unknown BLE state.";
            switch (CrossBluetoothLE.Current.State)
            {
                case BluetoothState.Unknown:
                    result = "Unknown BLE state.";
                    break;
                case BluetoothState.Unavailable:
                    result = "BLE is not available on this device.";
                    break;
                case BluetoothState.Unauthorized:
                    result = "You are not allowed to use BLE.";
                    break;
                case BluetoothState.TurningOn:
                    result = "BLE is warming up, please wait.";
                    break;
                case BluetoothState.On:
                    result = "BLE is on.";
                    break;
                case BluetoothState.TurningOff:
                    result = "BLE is turning off. That's sad!";
                    break;
                case BluetoothState.Off:
                    result = "BLE is off. Turn it on!";
                    break;
            }
            return result;
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Discovery(args.Device);
            //DiscoveryTest(args.Device);
        }

        private void OnDeviceAdvertised(object sender, DeviceEventArgs args)
        {
            Discovery(args.Device);
            //DiscoveryTest(args.Device);
        }

        private void Discovery(IDevice device) 
        {
            DeviceBluetooth deviceBluetooth = this.deviceBluetooth.FirstOrDefault(index => index.DeviceId == device.Id);
            if (deviceBluetooth == null)
            {
                Insert(device);
            }
            else
            {
                Update(device, deviceBluetooth);
            }
        }

        private void DiscoveryTest(IDevice device)
        {
            if (device != null)
            {
                this.adapterBluetooth.ConnectToDeviceAsync(device);
                Console.WriteLine($"Connected to device: {device.Name}");
                if (device.Name != null)
                    Console.WriteLine($"Connected to device: {device.Name}");
                var services = device.GetServicesAsync();
                Console.WriteLine($"Connected to service: {services}");
                foreach (var item in services.Result) 
                {
                    Console.WriteLine($"Connected to service: {item}");
                }
                /*
                CancellationToken cancellationToken;
                cancellationToken = new();
                for (int i = 0; i <= services.Result.Count; i++)
                {
                    var characteristics = services.Result[i].GetCharacteristicsAsync();
                    foreach (var item in characteristics.Result)
                    { 
                        (byte[], int) data = await item.ReadAsync();
                        Console.WriteLine($"characteristics: item 2 : {data.Item2} - item 1: {data.Item1}");
                    }
                }
                //var desiredService = services.FirstOrDefault();
                //var characteristics = await desiredService.GetCharacteristicsAsync();
                */
            }
        }
        public void Insert(IDevice device)
        {
            DeviceBluetooth local = new DeviceBluetooth();
            local.DeviceId = device.Id;
            local.Name = device.Name;
            local.Rssi = device.Rssi;
            local.IsConnectable = device.IsConnectable;
            local.AdvertisementRecords = device.AdvertisementRecords;
            local.State = device.State;
            local.Device = device;
            local.NativeDevice = device.NativeDevice.ToString();
            this.deviceBluetooth.Add(local);
        }

        public void Update(IDevice device, DeviceBluetooth local)
        {
            local.DeviceId = device.Id;
            local.Name = device.Name;
            local.Rssi = device.Rssi;
            local.IsConnectable = device.IsConnectable;
            local.AdvertisementRecords = device.AdvertisementRecords;
            local.State = device.State;
            local.Device = device;
            local.NativeDevice = device.NativeDevice.ToString();
        }

        public async void Stop()
        {
            await this.adapterBluetooth.StopScanningForDevicesAsync();
        }

        public async void Scan(CancellationTokenSource cancellationTokenSource)
        {
            this.deviceBluetooth = new ObservableCollection<DeviceBluetooth>();
            this.adapterBluetooth.DeviceAdvertised += OnDeviceAdvertised;
            this.adapterBluetooth.DeviceDiscovered += OnDeviceDiscovered;
            await UpdateConnectedDevices();

            cancellationTokenSource = new();
            await this.adapterBluetooth.StartScanningForDevicesAsync();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        private async Task UpdateConnectedDevices()
        {
            foreach (IDevice connectedDevice in this.adapterBluetooth.ConnectedDevices)
            {
                try
                {
                    await connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    throw new FileLoadException($"Exceção: {ex.Message}");
                }
                Discovery(connectedDevice);
            }
        }

        private IDevice device;

        public async Task<bool> Connect(Guid guid)
        {
            await this.adapterBluetooth.StopScanningForDevicesAsync();

            bool value = false;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    DeviceBluetooth deviceBluetooth = this.deviceBluetooth.FirstOrDefault(index => index.DeviceId == guid);
                    var parameters = new ConnectParameters(forceBleTransport: true);
                    CancellationTokenSource cancellationTokenSource;
                    cancellationTokenSource = new();
                    this.device = deviceBluetooth.Device;
                    await this.adapterBluetooth.ConnectToDeviceAsync(this.device, parameters, cancellationTokenSource.Token);
                    List<string> devices = new List<string>();
                    int count = this.adapterBluetooth.ConnectedDevices.Count;
                    for (int quantity = 0; quantity < count; quantity++)
                    {
                        devices.Add(this.adapterBluetooth.ConnectedDevices[quantity].Name);
                    }
                    IDevice device = this.adapterBluetooth.ConnectedDevices[0];
                    CancellationToken cancellationToken;
                    cancellationToken = new();
                    var service = await device.GetServicesAsync();
                    List<string> characteristics = new List<string>();
                    List<string> services = new List<string>();
                    count = service.Count;
                    for (int quantity = 0; quantity < count; quantity++)
                    {
                        services.Add(service[quantity].Name);
                        Task<IReadOnlyList<ICharacteristic>> characteristic = service[quantity].GetCharacteristicsAsync();
                        int count2 = characteristic.Result.Count;
                        for (int quantity2 = 0; quantity2 < count2; quantity2++)
                        {
                            try
                            {
                                if (characteristic.Result[quantity2].CanUpdate)
                                {
                                    characteristic.Result[quantity2].ValueUpdated += OnUpdatingValue;
                                    await characteristic.Result[quantity2].StartUpdatesAsync();
                                }
                                if (characteristic.Result[quantity2].CanWrite)
                                {
                                    byte[] dataToSend = Encoding.UTF8.GetBytes("Hello BLE!");
                                    await characteristic.Result[quantity2].WriteAsync(dataToSend);
                                }
                                //characteristic.Result[quantity2].ValueUpdated += OnUpdatingValue;
                                characteristics.Add(characteristic.Result[quantity2].Name);
                            }
                            catch (Exception ex)
                            { 
                            }
                        }
                    }
                    break;
/*
                        try
                        {
                            var characteristics = await item.GetCharacteristicsAsync();
                            var dev = device.Name;
                            var name = item.Device.Name;
*/
                            /*
                            foreach(var meu in characteristics)
                            {
                                try
                                {
                                    await meu.StartUpdatesAsync();

                                    if (meu.CanWrite)
                                    {
                                        byte[] dataToSend = Encoding.UTF8.GetBytes("Hello BLE!");
                                        await meu.WriteAsync(dataToSend);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
*/
/*
                        /*
                        string variavel = "";
                        foreach (var charact in characteristic)
                        {
                            variavel += charact.StringValue;
                        }
                        */
                }
                catch (Exception ex)
                {
                    Task.Delay(2000);
                }
            }
            return value;
        }

        public async Task Disconnect(Guid guid)
        {
            DeviceBluetooth deviceBluetooth = this.deviceBluetooth.FirstOrDefault(index => index.DeviceId == guid);
            await this.adapterBluetooth.DisconnectDeviceAsync(deviceBluetooth.Device);
        }

        public async Task<List<IService>> Read()
        {
            var req = this.device.RequestMtuAsync;
            var adv = this.device.AdvertisementRecords;
            foreach (var record in adv)
            {
                Console.WriteLine($"Advertisement Record Type: {record.Type}, Data: {BitConverter.ToString(record.Data)}");
            }

            var service = await this.device.GetServiceAsync(this.device.Id);

            if (service != null)
            {
                var characteristic = await service.GetCharacteristicAsync(service.Id);
                if (characteristic != null)
                {
                    var bytes = await characteristic.ReadAsync();

                    characteristic.ValueUpdated += (s, args) =>
                    {
                        var updatedBytes = args.Characteristic.Value;
                    };
                    await characteristic.StartUpdatesAsync();
                }
            }
            return null;
        }

        public List<Byte[]> characteristicByte { get; set; }

        private void OnUpdatingValue(object sender, CharacteristicUpdatedEventArgs args)
        {
            characteristicByte.Add(args.Characteristic.Value);
        }

        public async Task Read(Guid guid)
        {
            var service = await this.device.GetServiceAsync(guid);
            var characteristic = await service.GetCharacteristicAsync(guid);
            characteristic.ValueUpdated += OnUpdatingValue;
            await characteristic.StartUpdatesAsync();
        }

        public async Task Write(IDevice device, byte[] newData)
        {
            IReadOnlyList<IService> service = await device.GetServicesAsync();
            IService desiredService = service.FirstOrDefault(); // (s => s.Id == /* Your service UUID */)

            IReadOnlyList<ICharacteristic> characteristics = await desiredService.GetCharacteristicsAsync();
            ICharacteristic desiredCharacteristic = characteristics.FirstOrDefault(); // (c => c.Id == /* Your characteristic UUID */)

            await desiredCharacteristic.WriteAsync(newData);
        }

        public async Task Write(IDevice device)
        {
            IReadOnlyList<IService> service = await device.GetServicesAsync();
            IService desiredService = service.FirstOrDefault(); // (s => s.Id == /* Your service UUID */)

            IReadOnlyList<ICharacteristic> characteristics = await desiredService.GetCharacteristicsAsync();
            ICharacteristic desiredCharacteristic = characteristics.FirstOrDefault(); // (c => c.Id == /* Your characteristic UUID */)

            desiredCharacteristic.ValueUpdated += (s, e) => { /* Handle updated value */ };
            //await desiredCharacteristic.StartNotificationsAsync();
            await desiredCharacteristic.StartUpdatesAsync();
            await desiredCharacteristic.StopUpdatesAsync();
        }
    }
}
