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
    private HashSet<(int, int, int)> _moveRangePosition;
    private HashSet<(int, int, int)> _attackRangePosition;

    // TODO : Entity Stat
    private int _maxHp;
    private int _hp;
    private int _speed;
    private int _movePoint;
    private int _damage;
    private int _defense;
    private (int, int) _attackRange;

    public int Speed => _speed;
    
    public void Init()
    {
        _moveRangePosition = new HashSet<(int, int, int)>();
        _attackRangePosition = new HashSet<(int, int, int)>();
        Random random = new Random();
        
        _maxHp = 20;
        _hp = _maxHp;
        _speed = random.Next(100);
        _movePoint = 5;
        _damage = 10;
        _defense = 5;
        _attackRange = (1, 2);
    }
    
    public void Update()
    {
        _attackRangePosition = new HashSet<(int, int, int)>();
        _moveRangePosition = UpdateActionRange(_movePoint);
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
        ShowRange(_moveRangePosition);
    }

    public void ShowAttackRange()
    {
        _attackRangePosition = UpdateActionRange(_attackRange.Item1);

        ShowRange(_attackRangePosition);
    }
    
    public void Move(C_PlayerState packet)
    {
        (int, int, int) position = ((int, int, int))(packet.X, packet.Y, packet.Z);

        // 이동 가능한 위치인지 체크
        if (!_moveRangePosition.Contains(position))
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
        if (!_attackRangePosition.Contains(position))
            return;
        
        if (!MapManager.Instance.EntitiesOnMapDic.TryGetValue(position, out var target)) 
            return;

        int damage = Math.Max(_damage - target._defense, 0);
        target._hp -= damage;
        
        S_Attack attack = new S_Attack
        {
            PlayerId = Id,
            TargetId = target.Id,
            Damage = damage,
            MaxHp = target._maxHp,
            Hp = target._hp
        };

        Console.WriteLine($"target Hp : {target._hp}");
        // TODO : 사망 처리

        Room.Broadcast(attack.Write());

        State = EntityState.Waiting;
    }
}