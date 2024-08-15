using Server;
using ServerCore;

public class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_ReadyGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_ReadyGame readyPacket = packet as C_ReadyGame;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.ReadyGame(clientSession, readyPacket));
    }
    
    public static void C_EndTurnHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_EndTurn endTurnPacket = packet as C_EndTurn;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.EndTurn(clientSession));
    }
    
    public static void C_PlayerActionHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_PlayerAction actionPacket = packet as C_PlayerAction;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.ActionPlayer(clientSession, actionPacket));
    }
}