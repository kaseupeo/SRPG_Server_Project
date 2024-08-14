using ServerCore;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame enterGamePacket = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.EnterGame(enterGamePacket);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame leaveGamePacket = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.LeaveGame(leaveGamePacket);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList playerListPacket = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.Add(playerListPacket);
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove movePacket = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.Move(movePacket);
    }

    // 이동할 수 있는 위치 범위 리스트
    public static void S_ValidPositionHandler(PacketSession session, IPacket packet)
    {
        S_ValidPosition possibleMovePacket = packet as S_ValidPosition;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.FindValidPosition(possibleMovePacket);
    }

    // 이동 경로 리스트
    public static void S_MoveHandler(PacketSession session, IPacket packet)
    {
        S_Move movePacket = packet as S_Move;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.Move2(movePacket);
    }
}