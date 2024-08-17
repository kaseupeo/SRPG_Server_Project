using System.Net;
using ServerCore;

namespace Server;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public Entity Entity { get; set; }
    public bool IsReady { get; set; }

    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected : {endPoint}");

        Program.Room.Push(() => Program.Room.Enter(this));
    }

    public override void OnSend(int numOfBytes)
    {
        // Console.WriteLine($"Transferred bytes : {numOfBytes}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);
        if (Entity.Room != null)
        {
            GameRoom room = Entity.Room;
            room.Push(() => room.Leave(this));
            Entity.Room = null;
        }
        
        Console.WriteLine($"OnDisconnected : {endPoint}");
    }

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnReceivePacket(this, buffer);
    }
}