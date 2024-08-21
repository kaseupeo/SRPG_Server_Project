using ServerCore;

namespace Server;

public class GameRoom : IJobQueue
{
    private List<ClientSession> _sessionList = new List<ClientSession>();
    private JobQueue _jobQueue = new JobQueue();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    private List<Entity> _entityList = new List<Entity>();
    public TurnSystem TurnSystem = new TurnSystem();

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
        session.Entity = new Entity(session.SessionId);
        session.Entity.Room = this;
        
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
        
        // 맵 생성
        MapManager.Instance.GenerateMap();
        S_MapData mapData = new S_MapData();
        mapData.Width = MapManager.Instance.Width;
        mapData.Length = MapManager.Instance.Length;
        foreach (KeyValuePair<(int X, int Y, int Z),bool> pair in MapManager.Instance.Map)
        {
            mapData.mapList.Add(new S_MapData.Map()
            {
                X = pair.Key.X,
                Y = pair.Key.Y,
                Z = pair.Key.Z,
                Data = pair.Value
            });
        }

        Console.WriteLine("Map");
        Broadcast(mapData.Write());
        
        // 몬스터 생성
        // TODO : 몬스터 개수 나중에 빼기
        int enemyCount = 1;
        int enemyId = 10;
        Random random = new Random();
        // S_EnemyData enemyData = new S_EnemyData();
        // for (int i = 0; i < enemyCount; i++)
        // {
        //     enemyData.enemyList.Add(new S_EnemyData.Enemy()
        //     {
        //         EnemyId = enemyId++,
        //         X = random.Next(0, 10),
        //         Y = 0,
        //         Z = random.Next(0, 10)
        //     });
        // }
        //
        // Console.WriteLine("enemyData");
        // Broadcast(enemyData.Write());
        
        // 게임 시작
        S_StartGame startGame = new S_StartGame();
        
        // TEST : Enemy Create
        for (int i = 0; i < enemyCount; i++)
        {
            int id = enemyId++;
            Entity entity = new Entity(id);
            _entityList.Add(entity);
            entity.Init();
            TurnSystem.Add(entity);
            int x = random.Next(0, 10);
            int z = random.Next(0, 10);
            entity.Position = (x, 0, z);
            startGame.entityList.Add(new S_StartGame.Entity()
            {
                PlayerId = id,
                X = x,
                Y = 0,
                Z = z
            });
            Console.WriteLine($"enemy Create : {id}");
        }
        foreach (ClientSession clientSession in _sessionList)
        {
            // TEST : 테스트용 랜덤 속도 스탯 주기
            clientSession.Entity.Init();
            TurnSystem.Add(clientSession.Entity);
            
            startGame.entityList.Add(new S_StartGame.Entity()
            {
                PlayerId = clientSession.Entity.Id,
                X = 0,
                Y = 0,
                Z = 0,
            });
        }
        
        Console.WriteLine("startGame");
        Broadcast(startGame.Write());
        
        StartTurn();
    }

    public void StartTurn()
    {
        S_StartTurn startTurn = new S_StartTurn();
        startTurn.PlayerId = TurnSystem.CurrentTurn().Id;
        TurnSystem.CurrentTurn().Update();
        Broadcast(startTurn.Write());
    }
    
    public void EndTurn(ClientSession session)
    {
        if (session.SessionId != TurnSystem.CurrentTurn().Id)
            return;
        
        TurnSystem.NextTurn();
        StartTurn();
    }
    
    public void State(ClientSession session, C_PlayerState packet)
    {
        if (session.SessionId != TurnSystem.CurrentTurn().Id)
            return;

        session.Entity.ChangeState(packet);
    }
}