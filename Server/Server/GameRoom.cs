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
        session.EntityData = new EntityData(session.SessionId);
        
        // TODO 
        // 새로 들어온 플레이어한테 모든 플레이어 목록 전송
        S_PlayerList playerList = new S_PlayerList();
        foreach (ClientSession clientSession in _sessionList)
        {
            playerList.playerList.Add(new S_PlayerList.Player
            {
                IsSelf = (clientSession == session),
                PlayerId = clientSession.SessionId,
            });
        }

        session.Send(playerList.Write());
        
        // 새로 들어온 플레이어 입장을 모두에게 알림
        S_EnterGame enterGame = new S_EnterGame();
        enterGame.PlayerId = session.SessionId;
        
        Broadcast(enterGame.Write());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거
        _sessionList.Remove(session);
        
        // TODO 
        // 모두에게 알림
        S_LeaveGame leaveGame = new S_LeaveGame();
        leaveGame.PlayerId = session.SessionId;
        Broadcast(leaveGame.Write());
    }

    public void ReadyGame(ClientSession session, C_ReadyGame packet)
    {
        session.IsReady = packet.IsReady;
        
        // TODO : 준비 상태를 모두에게 알리는 패킷 만들면 추가
        S_ReadyGame readyGame = new S_ReadyGame();
        foreach (ClientSession clientSession in _sessionList)
        {
            readyGame.playerList.Add(new S_ReadyGame.Player()
            {
                IsReady = clientSession.IsReady,
                PlayerId = clientSession.SessionId,
            });
        }
        Broadcast(readyGame.Write());

        if (!_sessionList.All(x => x.IsReady)) 
            return;
        
        S_StartGame startGame = new S_StartGame();

        foreach (ClientSession clientSession in _sessionList)
        {
            // TEST : 테스트용 랜덤 속도 스탯 주기
            Random random = new Random();
            clientSession.EntityData.Speed = random.Next(100);
            _turnSystem.Add(clientSession.EntityData);
            
            startGame.entityList.Add(new S_StartGame.Entity()
            {
                PlayerId = clientSession.EntityData.Id,
                X = 0,
                Y = 0,
                Z = 0,
            });
        }
        
        Broadcast(startGame.Write());
        
        // MEMO : 그냥 딜레이 줌 
        Thread.Sleep(2000);
        
        S_StartTurn startTurn = new S_StartTurn();
        startTurn.PlayerId = _turnSystem.CurrentTurn().Id;
        Broadcast(startTurn.Write());
    }

    public void EndTurn(ClientSession session)
    {
        session.EntityData.ValidPosition = null;
        
        S_StartTurn startTurn = new S_StartTurn();
        _turnSystem.NextTurn();
        startTurn.PlayerId = _turnSystem.CurrentTurn().Id;
        Broadcast(startTurn.Write());
    }
    
    public void State(ClientSession session, C_PlayerState packet)
    {
        if (session.SessionId != _turnSystem.CurrentTurn().Id)
            return;
        
        switch ((PlayerAction)packet.State)
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
    
    public void ShowMoveRange(ClientSession session, C_PlayerState packet)
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
        validPosition.PlayerId = session.SessionId;
        
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
    
    public void Move(ClientSession session, C_PlayerState packet)
    {
        (int, int, int) position = ((int, int, int))(packet.X, packet.Y, packet.Z);

        // 이동 가능한 위치인지 체크
        if (!session.EntityData.ValidPosition.Contains(position))
            return;
        
        session.EntityData.Position = position;

        // TODO : 이동 가능하면 A* 알고리즘을 통해 경로 보내기
        // TEST : 간단한 테스트를 위해 받은 위치로 순간이동 시킴
        S_Move move = new S_Move();
        move.PlayerId = session.SessionId;
        move.pathList.Add(new S_Move.Path()
        {
            X = session.EntityData.Position.X,
            Y = session.EntityData.Position.Y,
            Z = session.EntityData.Position.Z,
        });

        Broadcast(move.Write());
    }
}