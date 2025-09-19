using net_maui_app_v24.Models;

namespace net_maui_app_v24.Interfaces
{
    public interface IDetectService
    {
        DetectResult Detect(byte[] file);
    }
}
