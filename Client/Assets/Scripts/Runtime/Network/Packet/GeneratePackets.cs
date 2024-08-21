
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    S_EnterGame = 1,
	C_LeaveGame = 2,
	S_LeaveGame = 3,
	S_PlayerList = 4,
	S_MapData = 5,
	S_EnemyData = 6,
	C_ReadyGame = 7,
	S_ReadyGame = 8,
	S_StartGame = 9,
	S_StartTurn = 10,
	C_EndTurn = 11,
	C_PlayerState = 12,
	S_ActionRange = 13,
	S_Move = 14,
	S_Attack = 15,
	S_Dead = 16,
	
}

public interface IPacket
{
	public ushort Protocol { get; }
	public void Read(ArraySegment<byte> segment);
	public ArraySegment<byte> Write();
}


public class S_EnterGame : IPacket
{
    public int PlayerId;
	public ushort Protocol => (ushort)PacketID.S_EnterGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_EnterGame);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
		count += sizeof(int);
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
public class S_LeaveGame : IPacket
{
    public int PlayerId;
	public ushort Protocol => (ushort)PacketID.S_LeaveGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_LeaveGame);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
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
	    public bool IsSelf;
		public int PlayerId;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.IsSelf = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
			count += sizeof(bool);
			
			this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.IsSelf);
			count += sizeof(bool);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
			count += sizeof(int);
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
public class S_MapData : IPacket
{
    public int Width;
	public int Length;
	
	public class Map
	{
	    public int X;
		public int Y;
		public int Z;
		public bool Data;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.X = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.Y = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.Z = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.Data = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
			count += sizeof(bool);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Data);
			count += sizeof(bool);
	        return success;
	    }
	}
	
	public List<Map> mapList = new List<Map>();
	
	public ushort Protocol => (ushort)PacketID.S_MapData;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.Width = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.Length = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.mapList.Clear();
		ushort mapLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < mapLength; i++)
		{
		    Map map = new Map();
		    map.Read(s, ref count);
		    mapList.Add(map);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_MapData);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Width);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Length);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)mapList.Count);
		count += sizeof(ushort);
		
		foreach (Map map in this.mapList) 
		    success &= map.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_EnemyData : IPacket
{
    
	public class Enemy
	{
	    public int EnemyId;
		public int X;
		public int Y;
		public int Z;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.EnemyId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.X = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.Y = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.Z = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.EnemyId);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.X);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Y);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Z);
			count += sizeof(int);
	        return success;
	    }
	}
	
	public List<Enemy> enemyList = new List<Enemy>();
	
	public ushort Protocol => (ushort)PacketID.S_EnemyData;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.enemyList.Clear();
		ushort enemyLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < enemyLength; i++)
		{
		    Enemy enemy = new Enemy();
		    enemy.Read(s, ref count);
		    enemyList.Add(enemy);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_EnemyData);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)enemyList.Count);
		count += sizeof(ushort);
		
		foreach (Enemy enemy in this.enemyList) 
		    success &= enemy.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class C_ReadyGame : IPacket
{
    public bool IsReady;
	public ushort Protocol => (ushort)PacketID.C_ReadyGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.IsReady = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_ReadyGame);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.IsReady);
		count += sizeof(bool);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_ReadyGame : IPacket
{
    
	public class Player
	{
	    public int PlayerId;
		public bool IsReady;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.IsReady = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
			count += sizeof(bool);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.IsReady);
			count += sizeof(bool);
	        return success;
	    }
	}
	
	public List<Player> playerList = new List<Player>();
	
	public ushort Protocol => (ushort)PacketID.S_ReadyGame;

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ReadyGame);
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
public class S_StartGame : IPacket
{
    
	public class Entity
	{
	    public int PlayerId;
		public float X;
		public float Y;
		public float Z;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        
			this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
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
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
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
	
	public List<Entity> entityList = new List<Entity>();
	
	public ushort Protocol => (ushort)PacketID.S_StartGame;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.entityList.Clear();
		ushort entityLength = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < entityLength; i++)
		{
		    Entity entity = new Entity();
		    entity.Read(s, ref count);
		    entityList.Add(entity);
		}
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_StartGame);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)entityList.Count);
		count += sizeof(ushort);
		
		foreach (Entity entity in this.entityList) 
		    success &= entity.Write(s, ref count);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_StartTurn : IPacket
{
    public int PlayerId;
	public ushort Protocol => (ushort)PacketID.S_StartTurn;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_StartTurn);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class C_EndTurn : IPacket
{
    
	public ushort Protocol => (ushort)PacketID.C_EndTurn;

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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_EndTurn);
        count += sizeof(ushort);
       
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class C_PlayerState : IPacket
{
    public int State;
	public float X;
	public float Y;
	public float Z;
	public ushort Protocol => (ushort)PacketID.C_PlayerState;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.State = BitConverter.ToInt32(s.Slice(count, s.Length - count));
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_PlayerState);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.State);
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
public class S_ActionRange : IPacket
{
    public int PlayerId;
	
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
	
	public ushort Protocol => (ushort)PacketID.S_ActionRange;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
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
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_ActionRange);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
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
    public int PlayerId;
	
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
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
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
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
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
public class S_Attack : IPacket
{
    public int PlayerId;
	public int TargetId;
	public int MaxHp;
	public int Hp;
	public int Damage;
	public ushort Protocol => (ushort)PacketID.S_Attack;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.TargetId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.MaxHp = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.Hp = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.Damage = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
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
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.TargetId);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.MaxHp);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Hp);
		count += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.Damage);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}
public class S_Dead : IPacket
{
    public int PlayerId;
	public ushort Protocol => (ushort)PacketID.S_Dead;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.PlayerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }
    
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Dead);
        count += sizeof(ushort);
       
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PlayerId);
		count += sizeof(int);
        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }
}