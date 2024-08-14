using System.Net;
using ServerCore;

namespace Server;

class Program
{
    private static Listener _listener = new Listener();
    public static GameRoom Room = new GameRoom();

    private static void FlushRoom()
    {
        Room.Push(() => Room.Flush());
        // MEMO : 패킷 전송 시간 밀리초
        // 30 프레임 : 1000 / 30
        JobTimer.Instance.Push(FlushRoom, 1000 / 30);
    }
    
    private static void Main(string[] args)
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddress = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

        _listener.Init(endPoint, () => SessionManager.Instance.Generate());
        Console.WriteLine("Listening...");

        // FlushRoom();
        JobTimer.Instance.Push(FlushRoom);
        
        while (true)
        {
            JobTimer.Instance.Flush();
        }
    }
}