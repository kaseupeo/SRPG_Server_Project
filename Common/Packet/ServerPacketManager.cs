
using System;
using System.Collections.Generic;
using ServerCore;

public class PacketManager
{
    #region Singleton

    private static PacketManager _instance = new PacketManager();
    public static PacketManager Instance => _instance;

    #endregion

    PacketManager() => Register();

    private Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new();
    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();

    public void Register()
    {

        _makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        _handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);

        _makeFunc.Add((ushort)PacketID.C_PlayerAction, MakePacket<C_PlayerAction>);
        _handler.Add((ushort)PacketID.C_PlayerAction, PacketHandler.C_PlayerActionHandler);

    }
    
    public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onReceiveCallback = null)
    {
        ushort count = 0;
        
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);

            if (onReceiveCallback != null)
                onReceiveCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }

    private T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);

        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action)) 
            action.Invoke(session, packet);
    }
}
