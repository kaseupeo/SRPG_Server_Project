using Server;
using Server.Content;
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

    public static void C_PlayerActionHandler(PacketSession session, IPacket packet)
    {
        C_PlayerAction actionPacket = packet as C_PlayerAction;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;

        switch ((PlayerAction)actionPacket.action)
        {
            case PlayerAction.ShowMoveRange:
                room.Push(() => room.ShowMoveRange(clientSession, actionPacket));
                break;
            case PlayerAction.Move:
                room.Push(() => room.Move(clientSession, actionPacket));
                break;
            default:
                break;
        }
    }
}