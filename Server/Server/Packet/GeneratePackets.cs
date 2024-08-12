
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    
}

public interface IPacket
{
	public ushort Protocol { get; }
	public void Read(ArraySegment<byte> segment);
	public ArraySegment<byte> Write();
}

