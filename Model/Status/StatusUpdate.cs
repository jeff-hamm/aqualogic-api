using System;

namespace AqualogicJumper.Services
{
    public class StatusUpdate
    {
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public bool IsFlashing { get; set; }

        public override string ToString()
        {
            return $"{Timestamp.ToString("O")}: {Text} - " + (IsFlashing ? "[Flashing]":"");
        }

    }
}