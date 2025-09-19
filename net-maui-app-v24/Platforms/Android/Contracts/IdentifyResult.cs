namespace net_maui_app_v24.Platforms.Android.Contracts
{
    public class IdentifyResult
    {
        public Guid FaceId
        {
            get; set;
        }
        public Candidate[] Candidates
        {
            get; set;
        }
    }
}
