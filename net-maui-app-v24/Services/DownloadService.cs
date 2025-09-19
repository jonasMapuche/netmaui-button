using CommunityToolkit.Maui.Storage;
using SharpCompress.Common;
using System.IO;
using System.Text;

namespace net_maui_app_v24.Services
{
    public class DownloadService
    {
        //private string URL = "http://192.168.0.3:8885/";
        private string URL = "http://api.stomach.com.br:8885/";

        private HttpClient httpClient;

        public DownloadService()
        {
            try
            {
                httpClient = new HttpClient();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> HttpPost(StreamContent message, string file_name)
        {
            try
            {
                string uri = URL + "File";
                using var content = new MultipartFormDataContent();
                content.Add(message, "\"fileUpload\"", $"{file_name}");
                using HttpResponseMessage response = await httpClient.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exceção: {ex.Message}");
                return null;
            }
        }
    }
}
