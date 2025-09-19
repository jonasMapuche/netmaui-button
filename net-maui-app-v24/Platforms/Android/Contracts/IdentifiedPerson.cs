namespace net_maui_app_v24.Platforms.Android.Contracts
{
    public class IdentifiedPerson
    {
        public double Confidence
        {
            get; set;
        }
        public Person Person
        {
            get; set;
        }
        public Guid FaceId
        {
            get; set;
        }
    }
}
