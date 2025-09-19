namespace net_maui_app_v24.Platforms.Android.Contracts
{
    public class Person
    {
        public Guid PersonId
        {
            get; set;
        }
        public Guid[] PersistedFaceIds
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string UserData
        {
            get; set;
        }
    }
}
