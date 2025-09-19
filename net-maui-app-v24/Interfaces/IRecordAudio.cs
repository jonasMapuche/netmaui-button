
namespace net_maui_app_v24.Interfaces
{
    public interface IRecordAudio
    {
        void StartRecord();
        string StopRecord();
        void StartRecordWav();
        string StopRecordWav();
    }
}
