using System;

namespace Maincotech.Messaging
{
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string EventName { get; set; }

        public string EventBody { get; set; }
    }
}