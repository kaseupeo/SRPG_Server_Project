using ServerCore;

namespace Server;

public class GameRoom : IJobQueue
{
    private List<ClientSession> _sessionList = new List<ClientSession>();
    private JobQueue _jobQueue = new JobQueue();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    private List<Entity> _entityList = new List<Entity>();
    public IReadOnlyList<Entity> EntityList => _entityList;
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
                EntityId = clientSession.SessionId,
            });
        }

        session.Send(playerList.Write());
        
        // 새로 들어온 플레이어 입장을 모두에게 알림
        S_EnterGame enterGame = new S_EnterGame();
        enterGame.EntityId = session.SessionId;
        
        Broadcast(enterGame.Write());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거
        _sessionList.Remove(session);
        
        // TODO 
        // 모두에게 알림
        S_LeaveGame leaveGame = new S_LeaveGame();
        leaveGame.EntityId = session.SessionId;
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
                EntityId = clientSession.SessionId,
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

        // TODO : 몬스터 개수 나중에 빼기
        int enemyCount = 1;
        int enemyId = 10;
        
        // 게임 시작
        S_StartGame startGame = new S_StartGame();

        foreach (ClientSession clientSession in _sessionList)
        {
            _entityList.Add(clientSession.Entity);
        }
        // 몬스터 생성
        for (int i = 0; i < enemyCount; i++)
        {
            int id = enemyId++;
            Entity entity = new Entity(id, EntityType.Enemy);
            entity.Room = this;
            _entityList.Add(entity);
        }

        foreach (Entity entity in _entityList)
        {
            entity.Init();
            TurnSystem.Add(entity);
            
            startGame.entityList.Add(new S_StartGame.Entity()
            {
                EntityId = entity.Id,
                Type = (int)entity.Type,
                X = entity.Position.X,
                Y = entity.Position.Y,
                Z = entity.Position.Z
            });
        }
        
        Console.WriteLine("startGame");
        Broadcast(startGame.Write());
        
        StartTurn();
    }

    public void StartTurn()
    {
        S_StartTurn startTurn = new S_StartTurn();
        startTurn.EntityId = TurnSystem.CurrentTurn().Id;

        switch (TurnSystem.CurrentTurn().Type)
        {
            case EntityType.Player:
                TurnSystem.CurrentTurn().Update();
                break;
            case EntityType.Enemy:
                TurnSystem.CurrentTurn().UpdateEnemy();
                break;
        }
        
        Broadcast(startTurn.Write());
    }
    
    public void EndTurn(ClientSession session)
    {
        if (session.SessionId != TurnSystem.CurrentTurn().Id)
            return;

        EndTurn();
    }

    public void EndTurn()
    {
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