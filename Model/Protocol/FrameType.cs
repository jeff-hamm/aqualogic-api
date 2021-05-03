using System;

namespace AqualogicJumper.Services
{
    public enum ControllerType : byte
    {
        LocalWired = 0x00,
        RemoteWired = 0x03, //0b00000011
        Wireless = 0x83,  //0B10000011
        Unknown = 0x8e,           //10001110
        RfRemote = 0x8c,       //10001100
    }

    public enum EventType
    {
        SendKey=0x00,

    }
    [Flags]
    public enum FrameType : Int16
    {

        LocalWiredKeyEvent = 0x00 << 8 | ControllerType.LocalWired,
        RemoteWiredKeyEvent = 0x00 << 8 | ControllerType.RemoteWired, //0b00000011
        WirelessKeyEvent = 0x00 << 8 | ControllerType.Wireless,  //0B10000011
        UnknownRemoteKey = 0x00 << 8 | ControllerType.Unknown,           //10001110
        RfRemoteKeyEvent = 0x00 << 8 | ControllerType.RfRemote,       //10001100

        OnOffEvent = 0x0005 ,

        KeepAlive =  0x0101 ,

        Switches = 0x0102 ,

        DisplayUpdate = 0x0103 ,

        LongDisplayUpdate = 0x040a ,

        PumpSpeedRequest = 0x0c01 ,

        PumpStatus = 0x000c ,

    }
}