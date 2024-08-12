using System.Net;
using ServerCore;

namespace Server;

public class ClientSession : PacketSession
{
    public int SessionID { get; set; }
    public GameRoom Room { get; set; }
    
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

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
        if (Room != null)
        {
            GameRoom room = Room;
            room.Push(() => room.Leave(this));
            Room = null;
        }
        
        Console.WriteLine($"OnDisconnected : {endPoint}");
    }

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnReceivePacket(this, buffer);
    }
}