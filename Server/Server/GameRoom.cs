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
                X = clientSession.PlayerData.Position.X,
                Y = clientSession.PlayerData.Position.Y,
                Z = clientSession.PlayerData.Position.Z,
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

    public void ShowMoveRange(ClientSession session, C_PlayerAction packet)
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

        // TODO : 맵 정보 구현 후 다시 정리
        Dictionary<(int, int, int), bool> map = new Dictionary<(int, int, int), bool>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                // 0이면 true (이동 가능), 1이면 false (장애물)
                map[(x, 0, z)] = grid[x, z] == 0;
            }
        }
        
        PathFind pathFind = new PathFind();
        HashSet<(int, int, int)> list = pathFind.FindTile(map, ((int)packet.X,(int)packet.Y, (int)packet.Z), 5);
        session.PlayerData.ValidPosition = list;

        
        S_MoveRange validPosition = new S_MoveRange();
        validPosition.playerID = session.SessionID;
        
        foreach ((int x, int y, int z) tuple in list)
        {
            validPosition.positionList.Add(new S_MoveRange.Position()
            {
                X = tuple.x,
                Y = tuple.y,
                Z = tuple.z,
            });
        }

        Broadcast(validPosition.Write());
    }
    
    public void Move(ClientSession session, C_PlayerAction packet)
    {
        (int, int, int) position = ((int, int, int))(packet.X, packet.Y, packet.Z);

        // 이동 가능한 위치인지 체크
        if (!session.PlayerData.ValidPosition.Contains(position))
            return;
        
        session.PlayerData.Position = position;

        // TODO : 이동 가능하면 A* 알고리즘을 통해 경로 보내기
        // TEST : 간단한 테스트를 위해 받은 위치로 순간이동 시킴
        S_Move move = new S_Move();
        move.playerID = session.SessionID;
        move.pathList.Add(new S_Move.Path()
        {
            X = session.PlayerData.Position.X,
            Y = session.PlayerData.Position.Y,
            Z = session.PlayerData.Position.Z,
        });

        Broadcast(move.Write());
    }
}