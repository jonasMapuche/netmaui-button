using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using net_maui_app_v24.Interfaces;
using net_maui_app_v24.Services;
using net_maui_app_v24.ViewModels;

namespace net_maui_app_v24
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel viewModel;
        public MainPage(IAudioPlayer audioPlayer, IRecordAudio recordAudio, ITextSpeak textSpeak, ICameraProvider cameraProvider)
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel(audioPlayer, recordAudio, textSpeak, cameraProvider);
            viewModel = (MainPageViewModel)BindingContext;
            viewModel.cameraView = Camera;

            _isGyroscopeAvailable = Gyroscope.Default.IsSupported;
            if (_isGyroscopeAvailable)
            {
                Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Aviso", "Giroscópio não suportado neste dispositivo.", "OK");
            }
            accelerometer = Accelerometer.Default;
            if (accelerometer.IsSupported)
            {
                accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                StartAccelerometer();
            }
            else 
            {
                Application.Current.MainPage.DisplayAlert("Aviso", "Acelerômetro não suportado neste dispositivo.", "OK");
            }

        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                lblX.Text = $"X: {e.Reading.Acceleration.X.ToString("0.00")}";
                lblY.Text = $"Y: {e.Reading.Acceleration.Y.ToString("0.00")}";
                lblZ.Text = $"Z: {e.Reading.Acceleration.Z.ToString("0.00")}";
            });
        }

        public void Shake_OnClick(object sender, EventArgs e)
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
                    Accelerometer.Default.Start(SensorSpeed.Game);
                }
                else
                {
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ShakeDetected -= Accelerometer_ShakeDetected;
                }
            }
        }

        public void Bluetooth_OnClick(object sender, EventArgs e)
        {
            bool state = BluetoothService.State;

            if (!state)
            {
                string message = BluetoothService.Message();
                Application.Current.MainPage.DisplayAlert("Aviso", message, "OK");
            } 
            else 
                Navigation.PushModalAsync(new ModalView());
        }

        private void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("Aviso", "Shake detected.", "OK");
        }

        private bool _isGyroscopeAvailable;
        private IAccelerometer accelerometer;
        private double _rotationThreshold = 0.5;

        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;

            if (data.AngularVelocity.Y > _rotationThreshold)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblGyroscope.Text = "Rotação para a direita";
                });
            }
            else if (data.AngularVelocity.Y < -_rotationThreshold)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblGyroscope.Text = "Rotação para a esquerda";
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblGyroscope.Text = "Sem rotação";
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_isGyroscopeAvailable)
            {
                Gyroscope.Default.Start(SensorSpeed.Default);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_isGyroscopeAvailable)
            {
                Gyroscope.Default.Stop();
            }
            if (accelerometer.IsSupported)
            {
                StopAccelerometer();
            }
        }

        private void StartAccelerometer()
        {
            try
            {
                if (accelerometer.IsSupported)
                {
                    accelerometer.Start(SensorSpeed.Default);
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Aviso", $"Erro ao iniciar o acelerômetro: {ex.Message}", "OK");
            }
        }

        private void StopAccelerometer()
        {
            if (accelerometer != null && accelerometer.IsMonitoring)
            {
                accelerometer.Stop();
            }
        }

        private void OnMediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            var memoryStream = new MemoryStream();
            e.Media.CopyTo(memoryStream);
            viewModel.Bytes = memoryStream.ToArray();
            viewModel.ShowCamera = false;
            viewModel.ShowPhoto = true;
            imgCamera.ScaleX = -1;
        }
    }
}
