using System.Runtime.Serialization;

namespace net_maui_app_v24.Platforms.Android.Contracts
{
    [DataContract]
    public class ClientError
    {
        public ClientError()
        {

        }
        [DataMember(Name = "error")]
        public ClientExceptionMessage Error
        {
            get;
            set;
        }
    }

    [DataContract]
    public class ClientExceptionMessage
    {
        [DataMember(Name = "code")]
        public string ErrorCode
        {
            get;
            set;
        }
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
    }
    [DataContract]
    public class ServiceError
    {
        public ServiceError()
        {
        }
        [DataMember(Name = "statusCode")]
        public string ErrorCode
        {
            get;
            set;
        }
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
    }
}