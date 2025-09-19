using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using net_maui_app_v24.Interfaces;
using net_maui_app_v24.Models;
using net_maui_app_v24.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace net_maui_app_v24.ViewModels
{
    public partial class MainPageViewModel: ObservableObject
    {
        [RelayCommand]
        private void SavePicture() 
        {
            ShowPhoto = false;
            ShowCamera = true;
        }

        public CancellationToken Token => CancellationToken.None;

        public ICommand PlayCommand { get; set; }
        public ICommand RecordCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand SpeakCommand { get; set; }
        public ICommand FileCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand UploadCommand { get; set; }
        public ICommand ConvertCommand { get; set; }
        public ICommand WavCommand { get; set; }
        public ICommand WavStopCommand { get; set; }
        public ICommand GPSCommand { get; set; }
        public ICommand RotateCommand { get; set; }
        public ICommand FlashCommand { get; set; }
        public ICommand FlashOffCommand { get; set; }
        public ICommand FlashAutoCommand { get; set; }
        public ICommand PikerCommand { get; set; }
        public ICommand CaptureCommand { get; set; }
        public ICommand PreviewCommand { get; set; }
        public ICommand StopPreviewCommand { get; set; }
        public ICommand EmguCVImageCommand { get; set; }
        public ICommand BatteryCommand { get; set; }
        public ICommand VibrationCommand { get; set; }
        public ICommand BluetoothCommand { get; set; }

        public string RecentAudioFilePath
        {
            get { return recentAudioFilePath; }
            set { recentAudioFilePath = value; }
        }

        public ObservableCollection<Audio> Audios
        {
            get { return audios; }
            set { audios = value; }
        }

        public Audio AudioFile
        {
            get { return audioFile; }
            set { audioFile = value; }
        }

        private readonly IRecordAudio recordAudio;
        private readonly IAudioPlayer audioPlayer;
        private readonly ITextSpeak textSpeak;
        private readonly ICameraProvider cameraProvider;
        private string recentAudioFilePath;
        private ObservableCollection<Audio> audios;
        private Audio audioFile;
        private AudioService audioService;

        [ObservableProperty]
        private CameraInfo selectedCamera;

        [ObservableProperty]
        private CameraFlashMode flashMode;

        [ObservableProperty]
        public bool showPhoto;

        [ObservableProperty]
        public bool showCamera;

        [ObservableProperty]
        private byte[]? bytes;

        private FaceService faceService;

        public CameraView cameraView;

        public MainPageViewModel(IAudioPlayer audioPlayer, IRecordAudio recordAudio, ITextSpeak textSpeak, ICameraProvider cameraProvider)
        {
            this.textSpeak = textSpeak;
            this.audioPlayer = audioPlayer;
            this.recordAudio = recordAudio;
            this.cameraProvider = cameraProvider;

            Audios = new ObservableCollection<Audio>();
            PlayCommand = new AsyncRelayCommand(OnPlayCommand);
            RecordCommand = new AsyncRelayCommand(OnRecordCommand);
            StopCommand = new AsyncRelayCommand(OnStopCommand);
            SpeakCommand = new AsyncRelayCommand(OnSpeakCommand);
            FileCommand = new AsyncRelayCommand(OnFileCommand);
            DownloadCommand = new AsyncRelayCommand(OnDownloadCommand);
            UploadCommand = new AsyncRelayCommand(OnUploadCommand);
            ConvertCommand = new AsyncRelayCommand(OnConvertCommand);
            WavCommand = new AsyncRelayCommand(OnWavCommand);
            WavStopCommand = new AsyncRelayCommand(OnWavStopCommand);
            GPSCommand = new AsyncRelayCommand(OnGPSCommand);
            RotateCommand = new AsyncRelayCommand(OnRotateCommand);
            FlashCommand = new AsyncRelayCommand(OnFlashCommand);
            FlashOffCommand = new AsyncRelayCommand(OnFlashOffCommand);
            FlashAutoCommand = new AsyncRelayCommand(OnFlashAutoCommand);
            PikerCommand = new AsyncRelayCommand(OnPickerCommand);
            CaptureCommand = new AsyncRelayCommand(OnCapatureCommand);
            PreviewCommand = new AsyncRelayCommand(OnPreviewCommand);
            StopPreviewCommand = new AsyncRelayCommand(OnStopPreviewCommand);
            EmguCVImageCommand = new AsyncRelayCommand(OnEmguCVImageCommand);
            BatteryCommand = new AsyncRelayCommand(OnBatteryCommand);
            VibrationCommand = new AsyncRelayCommand(OnVibrationCommand);
            BluetoothCommand = new AsyncRelayCommand(OnBluetoothCommand);

            audioService = new AudioService();

            ShowCamera = true;
            ShowPhoto = false;

            faceService = new FaceService();
        }

        private async Task OnBluetoothCommand()
        {
            await OnBluetoothDevice();
        }

        private async Task OnBluetoothDevice()
        {
            BluetoothService bluetoothService = new BluetoothService();
        }

        private async Task OnVibrationCommand()
        {
            await OnVibrationMobile();
        }

        private async Task OnVibrationMobile()
        {
            VibrationService vibrationService = new VibrationService();
            vibrationService.SetVibration(7);
        }

        private async Task OnBatteryCommand()
        {
            await OnBatteryCharge();
        }

        private async Task OnBatteryCharge()
        {
            BatteryService batteryService = new BatteryService();
            double battery = batteryService.GetCharge();
            Application.Current.MainPage.DisplayAlert("Aviso", "Battery " + battery + "%", "OK");
        }

        private async Task OnEmguCVImageCommand()
        {
            await OnEmguCVImage();
        }

        private async Task OnEmguCVImage()
        {
            FileResult result = await FilePicker.Default.PickAsync();

            if (result != null)
                if (result != null)
                {
                    using Stream sourceStream = await result.OpenReadAsync();
                    var memoryStream = new MemoryStream();
                    sourceStream.CopyTo(memoryStream);
                    Image output = new Image()
                    {
                        Source = ImageSource.FromStream(() => sourceStream)
                    };
                }
        }

        private async Task OnStopPreviewCommand()
        {
            await StopPreviewCamera();
        }

        private async Task StopPreviewCamera()
        {
            this.cameraView.StopCameraPreview();
        }

        private async Task OnPreviewCommand()
        {
            await StartPreviewCamera();
        }

        private async Task StartPreviewCamera()
        {
            await this.cameraView.StartCameraPreview(Token);
        }

        private async Task OnCapatureCommand()
        {
            await OnCaptureCamera();
        }

        private async Task OnCaptureCamera()
        {
            await this.cameraView.CaptureImage(Token);
        }

        private async Task OnPickerCommand()
        {
            await PickerMedia();
        }

        private string SetAudioFilePath(string value)
        {
            //string fileName = "/Record_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + value;
            string fileName = value;
            var path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string storagePath = path + fileName;
            Directory.CreateDirectory(path);
            return storagePath;
        }


        private async Task PickerMedia()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var options = new MediaPickerOptions
                    {
                        Title = "Capture a Photo"
                    };
                    FileResult fileResult = await MediaPicker.Default.CapturePhotoAsync(options);
                    if (fileResult is null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Aviso", "No photo captured", "OK");
                        return;
                    }

                    Stream photoStream = await fileResult.OpenReadAsync();

                    MemoryStream memoryStream = new MemoryStream();

                    photoStream.CopyTo(memoryStream);
                    Bytes = memoryStream.ToArray();
                    ShowCamera = false;
                    ShowPhoto = true;

                    DetectService detectService = new DetectService();
                    byte[] faces1 = net_maui_app_v24.Properties.Resources.faces1;
                    var detectResult1 = detectService.Detect(faces1);
                    //int detectResult = await faceService.Image1();

                    //FaceService faceService = new FaceService();
                    //int value = await faceService.Image1();

                    int detectResult = 0;

                    if (detectResult == null)
                        return;

                    if (detectResult > 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "Human Face Detected and no of face detected " + detectResult, "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "No Human Face Detected", "Ok");
                    }
                    /*
                    var path = SetAudioFilePath(fileResult.FileName);
                    using var fs = File.Create(path);
                    await photoStream.CopyToAsync(fs);
                    */
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }

        private async Task OnFlashOffCommand()
        {
            await FlashCamera("Off");
        }

        private async Task OnFlashAutoCommand()
        {
            await FlashCamera("Auto");
        }

        private async Task OnFlashCommand()
        {
            await FlashCamera("On");
        }

        private async Task FlashCamera(string kind)
        {
            if (cameraProvider.AvailableCameras is not null)
            {
                switch (kind)
                {
                    case "On":
                        FlashMode = CameraFlashMode.On;
                        break;
                    case "Off":
                        FlashMode = CameraFlashMode.Off;
                        break;
                    case "Auto":
                        FlashMode = CameraFlashMode.Auto;
                        break;
                }
            }
        }

        private async Task OnRotateCommand()
        {
            await RotateCamera();
        }

        private async Task RotateCamera()
        {
            if (cameraProvider.AvailableCameras is not null)
            {
                if (SelectedCamera.DeviceId == cameraProvider.AvailableCameras[0].DeviceId)
                    SelectedCamera = cameraProvider.AvailableCameras.First(x => x.Position == CameraPosition.Front);
                else if (SelectedCamera.DeviceId == cameraProvider.AvailableCameras[1].DeviceId)
                    SelectedCamera = cameraProvider.AvailableCameras.First(x => x.Position == CameraPosition.Rear);
            }
        }

        private async Task OnGPSCommand()
        {
            await LocationGPS();
        }

        private async Task LocationGPS()
        {
            GPSService gPSService = new GPSService();
            string result = await gPSService.GetCurrentLocation();
            Application.Current.MainPage.DisplayAlert("Aviso", result, "OK");
        }

        private async Task OnWavStopCommand()
        {
            await StopWav();
        }

        private async Task StopWav()
        {
            RecentAudioFilePath = recordAudio.StopRecordWav();
            SendRecording();
        }

        private async Task OnWavCommand()
        {
            await RecordWav();
        }
        private async Task RecordWav()
        {
            var permissionStatus = await RequestandCheckPermission();
            if (permissionStatus == PermissionStatus.Granted)
            {
                recordAudio.StartRecordWav();
            }
        }

        private async Task OnConvertCommand()
        {
            await ConvertMP3WAV();
        }

        private async Task ConvertMP3WAV()
        {
            audioFile = Audios.First();
            string audioFilePath = AudioFile.AudioURL;
            audioService.Converter(audioFilePath);
        }

        private async Task OnUploadCommand()
        {
            await UploadFile();
        }

        private async Task UploadFile()
        {
            RecentAudioFilePath = await audioService.UploadAudio();
            SendRecording();
        }

        private async Task OnDownloadCommand()
        {
            await DownloadFile();
        }

        private async Task DownloadFile()
        {
            audioFile = Audios.First();
            string audioFilePath = AudioFile.AudioURL;
            await audioService.DownloadAudio(audioFilePath);
        }

        private async Task OnFileCommand()
        {
            await FileText();
        }

        private async Task FileText()
        {
            RecentAudioFilePath = textSpeak.File("Hello World!");
            SendRecording();
        }

        private async Task OnSpeakCommand()
        {
            await SpeakText();
        }

        private async Task SpeakText()
        {
            textSpeak.Speak("Hello World!");
        }

        private async Task OnStopCommand()
        {
            await StopRecording();
        }

        private async Task OnRecordCommand()
        {
            await StartRecording();
        }

        private async Task OnPlayCommand()
        {
            await StartPlayingAudio();
        }

        private async Task StartRecording()
        {
            var permissionStatus = await RequestandCheckPermission();
            if (permissionStatus == PermissionStatus.Granted)
            {
                recordAudio.StartRecord();
            }
        }

        private async Task StopRecording()
        {
            RecentAudioFilePath = recordAudio.StopRecord();
            SendRecording();
        }

        private void SendRecording()
        {
            Audio recordedFile = new Audio() { AudioURL = RecentAudioFilePath };
            if (recordedFile != null)
            {
                recordedFile.AudioName = Path.GetFileName(RecentAudioFilePath);
                Audios.Insert(0, recordedFile);
            }
        }

        public async Task<PermissionStatus> RequestandCheckPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.StorageWrite>();

            status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.Microphone>();

            /*
            status = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.Sensors>();
            */

            PermissionStatus storagePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            PermissionStatus microPhonePermission = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            //PermissionStatus sensorsPermission = await Permissions.CheckStatusAsync<Permissions.Sensors>();

            //if (storagePermission == PermissionStatus.Granted && microPhonePermission == PermissionStatus.Granted && sensorsPermission == PermissionStatus.Granted)
            if (storagePermission == PermissionStatus.Granted && microPhonePermission == PermissionStatus.Granted)
            {
                return PermissionStatus.Granted;
            }
            return PermissionStatus.Denied;
        }

        private async Task StartPlayingAudio()
        {
            StopAudio();
            audioFile = Audios.First();
            string audioFilePath = AudioFile.AudioURL;
            audioPlayer.PlayAudio(audioFilePath);
        }

        public void StopAudio()
        {
            if (AudioFile != null)
            {
                audioPlayer.Stop();
            }
        }
    }
}
