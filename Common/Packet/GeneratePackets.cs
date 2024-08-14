
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_PlayerAction = 5,
	S_MoveRange = 6,
	S_Move = 7,
	S_AttackRange = 8,
	S_Attack = 9,
	
}

public interface IPacket
{
	public ushort Protocol { get; }
	public void Read(ArraySegment<byte> segment);
	public ArraySegment<byte> Write();
}


public class S_BroadcastEnterGame : IPacket
{
    public int playerID;
	public float X;
	public float Y;
	public float Z;
	public ushort Protocol => (ushort)PacketID.S_BroadcastEnterGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastEnterGame);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class C_LeaveGame : IPacket
{
    
	public ushort Protocol => (ushort)PacketID.C_LeaveGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_LeaveGame);
        count += sizeof(ushort);
       
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_BroadcastLeaveGame : IPacket
{
    public int playerID;
	public ushort Protocol => (ushort)PacketID.S_BroadcastLeaveGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastLeaveGame);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_PlayerList : IPacket
{
    
	public class Player
	{
	    public bool isSelf;
		public int playerID;
		public float X;
		public float Y;
		public float Z;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.isSelf = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
			count += sizeof(bool);
			
			this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.isSelf);
			count += sizeof(bool);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
			count += sizeof(float);
	        return success;
	    }
	}
	
	public List<Player> playerList = new List<Player>();
	
	public ushort Protocol => (ushort)PacketID.S_PlayerList;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerList.Clear();
		ushort playerLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < playerLength; i++)
		{
		    Player player = new Player();
		    player.Read(s, ref count);
		    playerList.Add(player);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_PlayerList);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)playerList.Count);
		count += sizeof(ushort);
		
		foreach (Player player in this.playerList) 
		    success &= player.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class C_PlayerAction : IPacket
{
    public int action;
	public float X;
	public float Y;
	public float Z;
	public ushort Protocol => (ushort)PacketID.C_PlayerAction;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.action = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_PlayerAction);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.action);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_MoveRange : IPacket
{
    public int playerID;
	
	public class Position
	{
	    public float X;
		public float Y;
		public float Z;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
			count += sizeof(float);
	        return success;
	    }
	}
	
	public List<Position> positionList = new List<Position>();
	
	public ushort Protocol => (ushort)PacketID.S_MoveRange;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.positionList.Clear();
		ushort positionLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < positionLength; i++)
		{
		    Position position = new Position();
		    position.Read(s, ref count);
		    positionList.Add(position);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_MoveRange);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)positionList.Count);
		count += sizeof(ushort);
		
		foreach (Position position in this.positionList) 
		    success &= position.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_Move : IPacket
{
    public int playerID;
	
	public class Path
	{
	    public float X;
		public float Y;
		public float Z;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
			count += sizeof(float);
	        return success;
	    }
	}
	
	public List<Path> pathList = new List<Path>();
	
	public ushort Protocol => (ushort)PacketID.S_Move;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.pathList.Clear();
		ushort pathLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < pathLength; i++)
		{
		    Path path = new Path();
		    path.Read(s, ref count);
		    pathList.Add(path);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Move);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)pathList.Count);
		count += sizeof(ushort);
		
		foreach (Path path in this.pathList) 
		    success &= path.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_AttackRange : IPacket
{
    public int playerID;
	
	public class Position
	{
	    public float X;
		public float Y;
		public float Z;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
			count += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
			count += sizeof(float);
	        return success;
	    }
	}
	
	public List<Position> positionList = new List<Position>();
	
	public ushort Protocol => (ushort)PacketID.S_AttackRange;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.positionList.Clear();
		ushort positionLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < positionLength; i++)
		{
		    Position position = new Position();
		    position.Read(s, ref count);
		    positionList.Add(position);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_AttackRange);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)positionList.Count);
		count += sizeof(ushort);
		
		foreach (Position position in this.positionList) 
		    success &= position.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_Attack : IPacket
{
    public int playerID;
	public float X;
	public float Y;
	public float Z;
	public ushort Protocol => (ushort)PacketID.S_Attack;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.X = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.Y = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.Z = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Attack);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerID);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
		count += sizeof(float);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}