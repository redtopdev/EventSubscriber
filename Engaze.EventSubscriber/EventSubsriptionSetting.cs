namespace Engaze.EventSubscriber.Service
{
    public class EventSubsriptionSetting
    {
        public string ConnString { get; set; }
        public string Stream { get; set; }
        public string SubscriptionGroup { get; set; }
        public int Buffersize { get; set; }
        public bool AutoAck { get; set; }
        public UserInfo User { get; set; }

        public class UserInfo
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }

    }
}
