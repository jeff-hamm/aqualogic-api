using System;

namespace AqualogicJumper.Services
{
    public class Frame
    {
        public DateTime StartTime { get; } = DateTime.Now;
        public short FrameCrc { get; set; }
        public FrameType Type { get; set; }
        public byte[] Body { get; set; }
        public byte[] Data { get; set; }
    }
}