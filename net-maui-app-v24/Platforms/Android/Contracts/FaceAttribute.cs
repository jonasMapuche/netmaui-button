using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace net_maui_app_v24.Platforms.Android.Helpers
{
    public class FaceAttribute
    {
        public double Age
        {
            get; set;
        }
        public string Gender
        {
            get; set;
        }
        public HeadPose HeadPose
        {
            get; set;
        }
        public double Smile
        {
            get; set;
        }
        public FacialHair FacialHair
        {
            get; set;
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public Glasses Glasses
        {
            get; set;
        }
    }
}
