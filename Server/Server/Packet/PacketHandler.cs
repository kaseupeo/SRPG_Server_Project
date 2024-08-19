using Server;
using ServerCore;

public class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Entity.Room == null)
            return;

        GameRoom room = clientSession.Entity.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_ReadyGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_ReadyGame readyPacket = packet as C_ReadyGame;

        if (clientSession.Entity.Room == null)
            return;

        GameRoom room = clientSession.Entity.Room;
        room.Push(() => room.ReadyGame(clientSession, readyPacket));
    }
    
    public static void C_EndTurnHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_EndTurn endTurnPacket = packet as C_EndTurn;

        if (clientSession.Entity.Room == null)
            return;

        GameRoom room = clientSession.Entity.Room;
        room.Push(() => room.EndTurn(clientSession));
    }
    
    public static void C_PlayerStateHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_PlayerState statePacket = packet as C_PlayerState;

        if (clientSession.Entity.Room == null)
            return;

        GameRoom room = clientSession.Entity.Room;
        room.Push(() => room.State(clientSession, statePacket));
    }
}