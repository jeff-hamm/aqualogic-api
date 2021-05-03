namespace AqualogicJumper.Services
{
    public static class AqualogicProtocolConstants
    {
        public const int FRAME_DLE = 0x10;

        public const int FRAME_STX = 0x02;

        public const int FRAME_ETX = 0x03;

        // DLE(1) + STX(1) + FrameType(2) + (1) + key(4) + keyRepeat(4) + (1) + crc(2) + DLE(1) + ETX(1)
        public const int KEY_FRAME_LENGTH = 1 + 1 + 2 + 1 + 4 + 4 + 1 + 2 + 1 + 1;
    }
}