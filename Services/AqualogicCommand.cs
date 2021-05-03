using System;

namespace AqualogicJumper.Services
{
    public abstract class AqualogicCommand
    {
        public AqualogicCommand()
        {
        }
        public bool Complete { get; private set; }
        public Key? LastKey { get; private set; }
        public bool HasNextKey(PoolStatusStore state, out Key key)
        {
            if(!Complete)
                Complete = IsComplete(state);
            if (Complete)
            {
                key = LastKey ?? Key.None;
                return false;
            }
            LastKey = NextKey(state);
            key = LastKey ?? Key.None;
            return key != Key.None;
        }

        protected abstract bool IsComplete(PoolStatusStore state);
        protected abstract Key NextKey(PoolStatusStore state);

    }
}