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

    // public static void C_MoveHandler(PacketSession session, IPacket packet)
    // {
    //     C_Move movePacket = packet as C_Move;
    //     ClientSession clientSession = session as ClientSession;
    //
    //     if (clientSession.Room == null)
    //         return;
    //
    //     GameRoom room = clientSession.Room;
    //     room.Push(() => room.Move(clientSession, movePacket));
    // }

    public static void C_StartGameHandler(PacketSession session, IPacket packet)
    {
        C_StartGame startGamePacket = packet as C_StartGame;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.FindValidPosition(clientSession, startGamePacket));
    }
    
    public static void C_ClickPositionHandler(PacketSession session, IPacket packet)
    {
        C_ClickPosition positionPacket = packet as C_ClickPosition;
        ClientSession clientSession = session as ClientSession;
        
        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.ClickPosition(clientSession, positionPacket));
    }
}