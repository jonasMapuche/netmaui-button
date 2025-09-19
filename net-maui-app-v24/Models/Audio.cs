
namespace net_maui_app_v24.Models
{
    public class Audio
    {
        private bool isPlayVisible;
        private bool isPauseVisible;
        private string currentAudioPostion;

        public Audio()
        {
            IsPlayVisible = true;
        }

        public string AudioName { get; set; }
        public string AudioURL { get; set; }
        public string Caption { get; set; }
        public bool IsPlayVisible
        {
            get { return isPlayVisible; }
            set
            {
                isPlayVisible = value;
                IsPauseVisble = !value;
            }
        }
        public bool IsPauseVisble
        {
            get { return isPauseVisible; }
            set { isPauseVisible = value; }
        }
        public string CurrentAudioPosition
        {
            get { return currentAudioPostion; }
            set
            {
                if (string.IsNullOrEmpty(currentAudioPostion))
                {
                    currentAudioPostion = string.Format("{0:mm\\:ss}", new TimeSpan());
                }
                else
                {
                    currentAudioPostion = value;
                }
            }
        }
    }
}
