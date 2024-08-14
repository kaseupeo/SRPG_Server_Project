using ServerCore;

namespace Server;

public class GameRoom : IJobQueue
{
    private List<ClientSession> _sessionList = new List<ClientSession>();
    private JobQueue _jobQueue = new JobQueue();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Flush()
    {
        foreach (ClientSession clientSession in _sessionList)
            clientSession.Send(_pendingList);

        // Console.WriteLine($"Flushed {_pendingList.Count} items");
        _pendingList.Clear();
    }
    
    public void Broadcast(ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }
    
    public void Enter(ClientSession session)
    {
        // 플레이어 추가
        _sessionList.Add(session);
        session.Room = this;
        
        // TODO 
        // 새로 들어온 플레이어한테 모든 플레이어 목록 전송
        S_PlayerList playerList = new S_PlayerList();
        foreach (ClientSession clientSession in _sessionList)
        {
            playerList.playerList.Add(new S_PlayerList.Player
            {
                isSelf = (clientSession == session),
                playerID = clientSession.SessionID,
                X = clientSession.PosX,
                Y = clientSession.PosY,
                Z = clientSession.PosZ,
            });
        }

        session.Send(playerList.Write());
        
        // 새로 들어온 플레이어 입장을 모두에게 알림
        S_BroadcastEnterGame enterGame = new S_BroadcastEnterGame();
        enterGame.playerID = session.SessionID;
        enterGame.X = 0;
        enterGame.Y = 0;
        enterGame.Z = 0;

        Broadcast(enterGame.Write());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거
        _sessionList.Remove(session);
        
        // TODO 
        // 모두에게 알림
        S_BroadcastLeaveGame leaveGame = new S_BroadcastLeaveGame();
        leaveGame.playerID = session.SessionID;
        Broadcast(leaveGame.Write());
    }

    // public void Move(ClientSession session, C_Move packet)
    // {
    //     // 좌표 바뀌기
    //     session.PosX = packet.X;
    //     session.PosY = packet.Y;
    //     session.PosZ = packet.Z;
    //     
    //     // 모두에게 알리기
    //     S_BroadcastMove move = new S_BroadcastMove();
    //     move.playerID = session.SessionID;
    //     move.X = session.PosX;
    //     move.Y = session.PosY;
    //     move.Z = session.PosZ;
    //
    //     Broadcast(move.Write());
    // }

    public void FindValidPosition(ClientSession session, C_StartGame packet)
    {
        // TODO : 나중에 맵 만들면 바꾸기
        int[,] grid = new int[10, 10]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            // { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            // { 0, 1, 1, 0, 0, 0, 0, 0, 1, 0 },
            // { 0, 0, 1, 0, 1, 1, 0, 0, 1, 0 },
            // { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            // { 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
            // { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            // { 0, 1, 0, 1, 1, 1, 0, 0, 1, 0 },
            // { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            // { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0 },
            // { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        Dictionary<(int, int), bool> map = new Dictionary<(int, int), bool>();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // 0이면 true (이동 가능), 1이면 false (장애물)
                map[(i, j)] = grid[i, j] == 0;
            }
        }
        
        PossibleMove possibleMove = new PossibleMove();
        HashSet<(int, int)> list = possibleMove.FindTile(map, ((int)packet.X, (int)packet.Z), 5);

        S_ValidPosition validPosition = new S_ValidPosition();
        validPosition.playerID = session.SessionID;

        foreach ((int x, int y) tuple in list)
        {
            validPosition.positionList.Add(new S_ValidPosition.Position()
            {
                X = tuple.x,
                Y = 0,
                Z = tuple.y,
            });

            Console.Write($"({tuple.x}, {tuple.y}), ");
        }

        Console.WriteLine("!!!");
        Broadcast(validPosition.Write());
    }
    
    public void ClickPosition(ClientSession session, C_ClickPosition packet)
    {
        // TODO : 이동 가능한 위치인지 체크
        // TEST : 간단한 테스트를 위해 체크 생략
        session.PosX = packet.X;
        session.PosY = packet.Y;
        session.PosZ = packet.Z;
        
        // TODO : 이동 가능하면 A* 알고리즘을 통해 경로 보내기
        // TEST : 간단한 테스트를 위해 받은 위치로 순간이동 시킴
        S_Move move = new S_Move();
        move.playerID = session.SessionID;
        move.pathList.Add(new S_Move.Path()
        {
            X = session.PosX,
            Y = session.PosY,
            Z = session.PosZ,
        });

        Broadcast(move.Write());
    }
}