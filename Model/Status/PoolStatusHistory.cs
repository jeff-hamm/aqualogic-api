using System.Collections.Generic;
using System.Linq;
using Cyotek.Collections.Generic;

namespace AqualogicJumper.Services
{
    public class PoolStatusHistory
    {
        public const int HistoryBufferSize = 128;
        private readonly CircularBuffer<StatusUpdate> _textBuffer = new CircularBuffer<StatusUpdate>(HistoryBufferSize);

        public ICollection<StatusUpdate> Text
        {
            get => _textBuffer;
            set
            {
                _textBuffer.Clear();
                foreach (var update in value)
                    _textBuffer.Append(update);
            }
        }

    }
}