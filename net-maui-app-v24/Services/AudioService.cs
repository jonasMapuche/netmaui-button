using FftSharp;
using System.Numerics;

namespace net_maui_app_v24.Services
{
    public class AudioService
    {

        private string SetAudioFilePath(string value)
        {
            string kind = value == "mp3" ? ".mp3" : ".wav";
            string fileName = "/Record_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + kind;
            var path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string storagePath = path + fileName;
            Directory.CreateDirectory(path);
            return storagePath;
        }

        public void AudioFFT(double[] audioData)
        {
            int sampleRate = 48_000;
            Complex[] spectrum = FFT.Forward(audioData);
            double[] psd = FFT.Power(spectrum);
            double[] freq = FFT.FrequencyScale(psd.Length, sampleRate);
        }

        public async Task DownloadAudio(string file_path)
        {
            FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            MemoryStream ms = new MemoryStream();
            await fs.CopyToAsync(ms);
            ms.Position = 0;
            using StreamContent streamContent = new StreamContent(ms);
            DownloadService downloadService = new DownloadService();
            await downloadService.HttpPost(streamContent, file_path);
        }

        public async Task<string> UploadAudio()
        {
            FileResult result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                using Stream sourceStream = await result.OpenReadAsync();
                string newFileName = "wav";
                string destinationPath = SetAudioFilePath(newFileName);

                using (FileStream destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
                return destinationPath;
            }
            return null;
        }

        internal void Converter(string audioFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
