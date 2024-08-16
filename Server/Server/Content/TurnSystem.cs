namespace Server;

public class TurnSystem
{
    private List<EntityData> _turnOrderBySpeed = new List<EntityData>();
    private int _order = 0;
    public IReadOnlyList<EntityData> TurnOrderBySpeed => _turnOrderBySpeed;

    public EntityData CurrentTurn()
    {
        if (_turnOrderBySpeed.Count <= _order)
            _order = 0;
        
        return _turnOrderBySpeed[_order];
    }
    
    public void NextTurn()
    {
        _order++;
    }
    
    private void Sort()
    {
        var finishedList = _turnOrderBySpeed.Take(_order).ToList();
        var newSortList = _turnOrderBySpeed.Skip(_order).ToList();

        newSortList.Sort((x, y) => y.Speed.CompareTo(x.Speed));
        
        _turnOrderBySpeed = finishedList.Concat(newSortList).ToList();        
        Console.WriteLine("순서");
        foreach (EntityData entity in _turnOrderBySpeed)
        {
            Console.WriteLine($"PlayerId : {entity.Id}, Speed : {entity.Speed}");
        }
    }
    
    public void Add(EntityData entityData)
    {
        _turnOrderBySpeed.Add(entityData);
        Sort();
    }

    public void Remove(EntityData entityData)
    {
        int index = _turnOrderBySpeed.IndexOf(entityData);
        
        if (index < _order) 
            _order--;

        _turnOrderBySpeed.RemoveAt(index);
        Sort();
    }
}