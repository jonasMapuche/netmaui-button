namespace net_maui_app_v24.Platforms.Android.Helpers
{
    public class Face
    {
        public Guid FaceId
        {
            get;
            set;
        }
        public FaceRectangle FaceRectangle
        {
            get;
            set;
        }
        public FaceLandmark FaceLandmarks
        {
            get;
            set;
        }
        public FaceAttribute FaceAttributes
        {
            get;
            set;
        }
    }
}
