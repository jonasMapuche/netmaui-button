using UltraFaceDotNet;

namespace net_maui_app_v24.Models
{
    public class DetectResult
    {
        public DetectResult(int width, int height, IEnumerable<FaceInfo> boxes)
        {
            this.Width = width;
            this.Height = height;
            this.Boxes = new List<FaceInfo>(boxes);
        }

        public IReadOnlyCollection<FaceInfo> Boxes
        {
            get;
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }
    }
}
