using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using net_maui_app_v24.Models;
using net_maui_app_v24.Services;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace net_maui_app_v24.ViewModels
{
    public partial class ModalViewModel : ObservableObject
    {
        BluetoothService bluetoothService;

        private IDispatcherTimer updateTimer;

        CancellationTokenSource _scanCancellationTokenSource = null;

        public ICommand BluetoothCommand { get; set; }

        public IDispatcherTimer UpdateTimer { get { return updateTimer; } set { updateTimer = value; } }

        [ObservableProperty]
        public ObservableCollection<DeviceBluetooth> bLEDevices;

        public ModalViewModel()
        {
            this.bluetoothService = new BluetoothService();
            this.bluetoothService.Scan(_scanCancellationTokenSource);
            Timer();
            BluetoothCommand = new AsyncRelayCommand<Guid>(OnBluetoothCommand);
        }

        private async Task OnBluetoothCommand(Guid guid)
        {
            await BluetoothDevice(guid);
        }

        private async Task BluetoothDevice(Guid guid)
        {
            bool value = await this.bluetoothService.Connect(guid);
            if (value)
            {
                await this.bluetoothService.Read(guid);
                await Application.Current.MainPage.DisplayAlert("Bluetooth", "Not Read", "OK");
            } 
            else
                Application.Current.MainPage.DisplayAlert("Bluetooth", "Not Connected!", "OK");
        }

        private void UpdateData(object sender, EventArgs e)
        {
            BLEDevices = this.bluetoothService.deviceBluetooth;
        }

        public void Timer()
        {
            this.updateTimer = Application.Current.Dispatcher.CreateTimer();
            this.updateTimer.Interval = TimeSpan.FromSeconds(5); 
            this.updateTimer.Tick += UpdateData;
            this.updateTimer.Start();
        }


    }
}
