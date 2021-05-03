using System;

namespace AqualogicJumper.Services
{
    [Flags]
    public enum MessageState
    {
        Pending = 0,
        Sent = 0b00001,
        Success = 0b00010,
        Timeout = 0b00100,
        Error = 0b01000,
        Retrying = 0b10001
    }
}