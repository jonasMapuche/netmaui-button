using Android.Speech.Tts;
using net_maui_app_v24.Interfaces;
using TextToSpeech = Android.Speech.Tts.TextToSpeech;

namespace net_maui_app_v24.Platforms.Android.Services
{
    public class TextSpeak : Java.Lang.Object, ITextSpeak, TextToSpeech.IOnInitListener
    {
        private TextToSpeech textToSpeech;
        private string storagePath;
        string toSpeak;

        public TextSpeak()
        {
            this.textToSpeech = new TextToSpeech(Platform.AppContext, this);
        }

        public void Speak(string text)
        {
            this.toSpeak = text;
            if (this.textToSpeech != null && this.textToSpeech.IsSpeaking == false)
            {
                this.textToSpeech.Speak(toSpeak, QueueMode.Flush, null, null);
            }
        }

        public string File(string text)
        {
            this.toSpeak = text;
            OperationResult result = OperationResult.Error;
            if (this.textToSpeech != null && this.textToSpeech.IsSpeaking == false)
            {
                SetAudioFilePath();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add(TextToSpeech.Engine.KeyParamUtteranceId, "fileSynthesis");

                result = this.textToSpeech.SynthesizeToFile(toSpeak, parameter, this.storagePath);
            }
            if (result == OperationResult.Success) return this.storagePath;
            return null;
        }

        public void OnInit(OperationResult status)
        {
            if (status == OperationResult.Success)
            {
                if (!string.IsNullOrEmpty(toSpeak))
                {
                    this.textToSpeech.Speak(toSpeak, QueueMode.Flush, null, null);
                }
            }
            else
            {
                Console.WriteLine("TTS Initialization failed");
            }
        }

        private void SetAudioFilePath()
        {
            string fileName = "/Record_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + ".mp3";
            //string fileName = "/Record.mp3";
            var path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            this.storagePath = path + fileName;
            Directory.CreateDirectory(path);
        }

    }
}
