using ServerCore;

namespace Server;

public class GameRoom : IJobQueue
{
    private List<ClientSession> _sessionList = new List<ClientSession>();
    private JobQueue _jobQueue = new JobQueue();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    private TurnSystem _turnSystem = new TurnSystem();
    
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
        session.EntityData = new EntityData(session.SessionID);
        _turnSystem.Add(session.EntityData);
        
        // TODO 
        // 새로 들어온 플레이어한테 모든 플레이어 목록 전송
        S_PlayerList playerList = new S_PlayerList();
        foreach (ClientSession clientSession in _sessionList)
        {
            playerList.playerList.Add(new S_PlayerList.Player
            {
                isSelf = (clientSession == session),
                playerID = clientSession.SessionID,
                X = clientSession.EntityData.Position.X,
                Y = clientSession.EntityData.Position.Y,
                Z = clientSession.EntityData.Position.Z,
            });
        }

        session.Send(playerList.Write());
        
        // 새로 들어온 플레이어 입장을 모두에게 알림
        S_BroadcastEnterGame enterGame = new S_BroadcastEnterGame();
        enterGame.playerID = session.SessionID;
        enterGame.X = 0;
        enterGame.Y = 0;
        enterGame.Z = 0;
        
        // TEST : 테스트용 랜덤 속도 스탯 주기
        Random random = new Random();
        enterGame.Speed = random.Next(100);
        session.EntityData.Speed = enterGame.Speed;

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

    public void ReadyGame(ClientSession session, C_ReadyGame packet)
    {
        session.IsReady = packet.IsReady;
        
        // TODO : 준비 상태를 모두에게 알리는 패킷 만들면 추가


        if (!_sessionList.All(x => x.IsReady)) 
            return;
        
        S_StartGame startGame = new S_StartGame();
        Broadcast(startGame.Write());

        _turnSystem.Sort();
        
        Thread.Sleep(2000);
        S_StartTurn startTurn = new S_StartTurn();
        startTurn.playerID = _turnSystem.EntityQueue.Peek().Id;
        Broadcast(startTurn.Write());
    }

    public void EndTurn(ClientSession session)
    {
        _turnSystem.EntityQueue.Dequeue();
        
        if (_turnSystem.EntityQueue.Count <= 0) 
            _turnSystem.Sort();
        
        S_StartTurn startTurn = new S_StartTurn();
        startTurn.playerID = _turnSystem.EntityQueue.Peek().Id;
        Broadcast(startTurn.Write());
    }
    
    public void ActionPlayer(ClientSession session, C_PlayerAction packet)
    {
        if (session.SessionID != _turnSystem.EntityQueue.Peek().Id)
            return;
        
        switch ((PlayerAction)packet.action)
        {
            case PlayerAction.ShowMoveRange:
                ShowMoveRange(session, packet);
                break;
            case PlayerAction.Move:
                Move(session, packet);
                break;
            case PlayerAction.ShowAttackRange:
                break;
            case PlayerAction.Attack:
                break;
            default:
                break;
        }
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
        session.EntityData.ValidPosition = list;

        
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
        if (!session.EntityData.ValidPosition.Contains(position))
            return;
        
        session.EntityData.Position = position;

        // TODO : 이동 가능하면 A* 알고리즘을 통해 경로 보내기
        // TEST : 간단한 테스트를 위해 받은 위치로 순간이동 시킴
        S_Move move = new S_Move();
        move.playerID = session.SessionID;
        move.pathList.Add(new S_Move.Path()
        {
            X = session.EntityData.Position.X,
            Y = session.EntityData.Position.Y,
            Z = session.EntityData.Position.Z,
        });

        Broadcast(move.Write());
    }
}