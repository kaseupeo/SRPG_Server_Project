using ServerCore;

public class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IPacket packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.EnterGame(enterGamePacket);
    }

    public static void S_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.LeaveGame(leaveGamePacket);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList playerListPacket = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.EnterGame(playerListPacket);
    }

    public static void S_ReadyGameHandler(PacketSession session, IPacket packet)
    {
        S_ReadyGame readyGamePacket = packet as S_ReadyGame;
        ServerSession serverSession = session as ServerSession;

        Managers.Game.ReadyGame(readyGamePacket);
    }
    
    public static void S_StartGameHandler(PacketSession session, IPacket packet)
    {
        S_StartGame startGamePacket = packet as S_StartGame;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.StartGame(startGamePacket);
    }
    
    public static void S_StartTurnHandler(PacketSession session, IPacket packet)
    {
        S_StartTurn startTurnPacket = packet as S_StartTurn;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.StartTurn(startTurnPacket);
    }
    
    // 이동할 수 있는 위치 범위 리스트
    public static void S_MoveRangeHandler(PacketSession session, IPacket packet)
    {
        S_MoveRange moveRangePacket = packet as S_MoveRange;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.ShowMoveRange(moveRangePacket);
    }

    // 이동 경로 리스트
    public static void S_MoveHandler(PacketSession session, IPacket packet)
    {
        S_Move movePacket = packet as S_Move;
        ServerSession serverSession = session as ServerSession;
        
        Managers.Game.Move(movePacket);
    }

    public static void S_AttackRangeHandler(PacketSession session, IPacket packet)
    {
        S_AttackRange attackRangePacket = packet as S_AttackRange;
        ServerSession serverSession = session as ServerSession;
     
        // TODO : 공격 범위 보여주는 함수 
        Managers.Game.ShowAttackRange(attackRangePacket);
    }
    
    public static void S_AttackHandler(PacketSession session, IPacket packet)
    {
        S_Attack attackPacket = packet as S_Attack;
        ServerSession serverSession = session as ServerSession;
     
        // TODO : 공격 함수 
        Managers.Game.Attack(attackPacket);
    }
}