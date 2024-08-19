namespace Server;

public class Entity(int id)
{
    // Entity ID
    public int Id { get; set; } = id;
    
    public GameRoom Room { get; set; }

    public EntityState State { get; set; }

    // Entity Position
    private (int X, int Y, int Z) _position;
    public (int X, int Y, int Z) Position
    {
        get => _position;
        set
        {
            MapManager.Instance.EntitiesOnMapDic.Remove(_position);
            if (!MapManager.Instance.EntitiesOnMapDic.ContainsKey(value))
            {
                MapManager.Instance.EntitiesOnMapDic.Add(value, this);
            }
            _position = value;
        }
    }

    // Entity MoveRange Position
    public HashSet<(int, int, int)> MoveRangePosition { get; set; } = new HashSet<(int, int, int)>();
    public HashSet<(int, int, int)> AttackRangePosition { get; set; } = new HashSet<(int, int, int)>();

    // TODO : Entity Stat
    public int Hp { get; set; } = 20;
    public int Speed { get; set; }
    public int MovePoint { get; set; } = 5;
    public int Damage { get; set; } = 10;
    public int Defense { get; set; } = 5;
    public (int, int) AttackRange { get; set; } = (1, 2);

    
    public void Update()
    {
        AttackRangePosition = new HashSet<(int, int, int)>();
        MoveRangePosition = UpdateActionRange(MovePoint);
    }

    public void ChangeState(C_PlayerState packet)
    {
        State = (EntityState)packet.State;
        switch (State)
        {
            case EntityState.ShowRange | EntityState.Move:
                ShowMoveRange();
                break;
            case EntityState.Move:
                Move(packet);
                break;
            case EntityState.ShowRange | EntityState.Attack:
                ShowAttackRange();
                break;
            case EntityState.Attack:
                Attack(packet);
                break;
            default:
                break;
        }
    }
    
    private HashSet<(int, int, int)> UpdateActionRange(int range)
    {
        PathFind pathFind = new PathFind();
        HashSet<(int, int, int)> list = pathFind.FindTile(MapManager.Instance.Map, (_position.X, _position.Y, _position.Z), range);

        return list;
    }

    private void ShowRange(HashSet<(int, int, int)> range)
    {
        S_ActionRange moveRange = new S_ActionRange();
        moveRange.PlayerId = Id;
        
        foreach ((int x, int y, int z) tuple in range)
        {
            moveRange.positionList.Add(new S_ActionRange.Position()
            {
                X = tuple.x,
                Y = tuple.y,
                Z = tuple.z,
            });
        }

        Room.Broadcast(moveRange.Write());
    }
    
    public void ShowMoveRange()
    {
        ShowRange(MoveRangePosition);
    }

    public void ShowAttackRange()
    {
        AttackRangePosition = UpdateActionRange(AttackRange.Item1);

        ShowRange(AttackRangePosition);
    }
    
    public void Move(C_PlayerState packet)
    {
        (int, int, int) position = ((int, int, int))(packet.X, packet.Y, packet.Z);

        // 이동 가능한 위치인지 체크
        if (!MoveRangePosition.Contains(position))
            return;
        
        Position = position;

        // TODO : 이동 가능하면 A* 알고리즘을 통해 경로 보내기
        // TEST : 간단한 테스트를 위해 받은 위치로 순간이동 시킴
        S_Move move = new S_Move();
        move.PlayerId = Id;
        move.pathList.Add(new S_Move.Path()
        {
            X = _position.X,
            Y = _position.Y,
            Z = _position.Z,
        });
        
        Room.Broadcast(move.Write());
    }
    
    public void Attack(C_PlayerState packet)
    {
        (int, int, int) position = ((int, int, int))(packet.X, packet.Y, packet.Z);
        
        // 공격범위에 있는지 체크
        if (!AttackRangePosition.Contains(position))
            return;
        
        if (!MapManager.Instance.EntitiesOnMapDic.TryGetValue(position, out var target)) 
            return;

        int damage = Math.Max(Damage - target.Defense, 0);
        target.Hp -= damage;
        
        S_Attack attack = new S_Attack
        {
            PlayerId = Id,
            TargetId = target.Id,
            Damage = damage,
            Hp = target.Hp
        };

        Console.WriteLine($"target Hp : {target.Hp}");
        // TODO : 사망 처리

        Room.Broadcast(attack.Write());

        State = EntityState.Waiting;
    }
}