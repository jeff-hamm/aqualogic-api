using System;
using System.Collections.Generic;

namespace AqualogicJumper.Services
{

    public class Message
    {
        public Key Key { get; set; }
        public byte[] Frame { get; set; }
        public int Attempts { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTime? FirstAttemptTime { get; set; }
        public DateTime? LastAttemptTime { get; set; }
        public MessageState State { get; set; } = MessageState.Pending;
        public Func<bool> Validate { get; set; }
        public Action<Message> Complete { get; set; }
    }
}