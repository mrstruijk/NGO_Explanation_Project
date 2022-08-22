using System;

/// <summary>
/// From: https://www.youtube.com/playlist?list=PLQMQNmwN3FvyyeI1-bDcBPmZiSaDMbFTi
/// </summary>
public struct RelayJoinData
{
    public string JoinCode;
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] HostConnectionData;
    public byte[] Key;
}