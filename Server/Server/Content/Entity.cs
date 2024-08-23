namespace Server;

public class Entity(int id, EntityType type = EntityType.Player)
{
    // Entity ID
    public int Id { get; set; } = id;
    public EntityType Type { get; set; } = type;
    public GameRoom Room { get; set; }
    public EntityState State { get; set; }
    // public bool IsPlayer { get; set; } = isPlayer;

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
    private (int min, int max) _attackRange;

    public int Speed => _speed;
    
    public void Init()
    {
        _moveRangePosition = new HashSet<(int, int, int)>();
        _attackRangePosition = new HashSet<(int, int, int)>();
        Random random = new Random();

        Position = MapManager.Instance.RandomMapPosition();
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

    public void UpdateEnemy()
    {
        Update();

        List<Entity> list = Room.EntityList.Where(e => e.Type == EntityType.Player).ToList();
        HashSet<(int, int, int)> range = new HashSet<(int, int, int)>();
        
        foreach (Entity target in list) 
            range.UnionWith(UpdateActionRange(target.Position, _attackRange.min));

        var set = _moveRangePosition.Intersect(range).ToList();

        if (set.Count == 0)
        {
            Console.WriteLine("enemy no move end turn");
            State = EntityState.Waiting;
            Room.EndTurn();
            return;
        }
        
        Move(set[0]);
        
        _attackRangePosition = UpdateActionRange(_attackRange.min);

        foreach (Entity entity in list)
        {
            if (_attackRangePosition.Contains(entity.Position))
            {
                Attack(entity.Position);
                Room.EndTurn();
                return;
            }
        }
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
                Move(((int, int, int))(packet.X, packet.Y, packet.Z));
                break;
            case EntityState.ShowRange | EntityState.Attack:
                ShowAttackRange();
                break;
            case EntityState.Attack:
                Attack(((int, int, int))(packet.X, packet.Y, packet.Z));
                break;
            default:
                break;
        }
    }
    
    private HashSet<(int, int, int)> UpdateActionRange(int range)
    {
        return UpdateActionRange((_position.X, _position.Y, _position.Z), range);
    }
    
    private HashSet<(int, int, int)> UpdateActionRange((int, int, int) position, int range)
    {
        PathFind pathFind = new PathFind();
        HashSet<(int, int, int)> list = pathFind.FindTile(position, range);

        return list;
    }

    private void ShowRange(HashSet<(int, int, int)> range)
    {
        S_ActionRange moveRange = new S_ActionRange();
        moveRange.EntityId = Id;
        
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

    private void ShowMoveRange()
    {
        ShowRange(_moveRangePosition);
    }

    private void ShowAttackRange()
    {
        _attackRangePosition = UpdateActionRange(_attackRange.min);

        ShowRange(_attackRangePosition);
    }
    
    private void Move((int, int, int) position)
    {
        // 이동 가능한 위치인지 체크
        if (!_moveRangePosition.Contains(position))
            return;
        
        Dictionary<(int, int, int), bool> map = new Dictionary<(int, int, int), bool>();
        foreach (var item in _moveRangePosition) 
            map[item] = true;
        
        // MEMO : ASTAR
        AStar aStar = new AStar(map);
        List<(int x, int y, int z)> path = aStar.FindPath(_position, position);
        Position = position;

        S_Move move = new S_Move();
        move.EntityId = Id;

        Console.WriteLine("path : ");
        foreach ((int x, int y, int z) tuple in path)
        {
            move.pathList.Add(new S_Move.Path()
            {
                X = tuple.x,
                Y = tuple.y,
                Z = tuple.z,
            });
            Console.WriteLine($"({tuple.x}, {tuple.y}, {tuple.z}), ");
        }
        
        Room.Broadcast(move.Write());
    }
    
    private void Attack((int, int, int) position)
    {
        Console.WriteLine("Attack In");
        
        // 공격범위에 있는지 체크
        if (!_attackRangePosition.Contains(position))
            return;

        Console.WriteLine("Attack Range In");
        if (!MapManager.Instance.EntitiesOnMapDic.TryGetValue(position, out var target)) 
            return;

        Console.WriteLine("Attack start");
        int damage = Math.Max(_damage - target._defense, 0);
        target._hp -= damage;
        
        S_Attack attack = new S_Attack
        {
            EntityId = Id,
            TargetId = target.Id,
            Damage = damage,
            MaxHp = target._maxHp,
            Hp = target._hp
        };

        Console.WriteLine($"target Hp : {target._hp}");
        // TODO : 사망 처리
        if (target._hp <= 0)
            target.Dead();

        Room.Broadcast(attack.Write());

        State = EntityState.Waiting;
    }

    private void Dead()
    {
        Room.TurnSystem.Remove(this);

        S_Dead dead = new S_Dead();
        dead.EntityId = Id;

        Room.Broadcast(dead.Write());
    }
}