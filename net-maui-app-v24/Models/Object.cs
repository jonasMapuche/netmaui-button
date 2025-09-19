using NcnnDotNet.OpenCV;

namespace net_maui_app_v24.Models
{
    public sealed class Object
    {

        public Object()
        {
            this.Rect = new Rect<float>();
        }

        public Rect<float> Rect
        {
            get;
            set;
        }

        public int Label
        {
            get;
            set;
        }

        public float Prob
        {
            get;
            set;
        }

    }
}
