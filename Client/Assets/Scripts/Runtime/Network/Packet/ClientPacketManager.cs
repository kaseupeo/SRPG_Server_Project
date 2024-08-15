
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

        _makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
        _handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);

        _makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        _handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);

        _makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
        _handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);

        _makeFunc.Add((ushort)PacketID.S_StartGame, MakePacket<S_StartGame>);
        _handler.Add((ushort)PacketID.S_StartGame, PacketHandler.S_StartGameHandler);

        _makeFunc.Add((ushort)PacketID.S_StartTurn, MakePacket<S_StartTurn>);
        _handler.Add((ushort)PacketID.S_StartTurn, PacketHandler.S_StartTurnHandler);

        _makeFunc.Add((ushort)PacketID.S_MoveRange, MakePacket<S_MoveRange>);
        _handler.Add((ushort)PacketID.S_MoveRange, PacketHandler.S_MoveRangeHandler);

        _makeFunc.Add((ushort)PacketID.S_Move, MakePacket<S_Move>);
        _handler.Add((ushort)PacketID.S_Move, PacketHandler.S_MoveHandler);

        _makeFunc.Add((ushort)PacketID.S_AttackRange, MakePacket<S_AttackRange>);
        _handler.Add((ushort)PacketID.S_AttackRange, PacketHandler.S_AttackRangeHandler);

        _makeFunc.Add((ushort)PacketID.S_Attack, MakePacket<S_Attack>);
        _handler.Add((ushort)PacketID.S_Attack, PacketHandler.S_AttackHandler);

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
