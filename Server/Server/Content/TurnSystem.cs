namespace Server;

public class TurnSystem
{
    private List<Entity> _turnOrderBySpeed = new List<Entity>();
    private int _order = 0;
    public IReadOnlyList<Entity> TurnOrderBySpeed => _turnOrderBySpeed;

    public Entity CurrentTurn()
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
        foreach (Entity entity in _turnOrderBySpeed)
        {
            Console.WriteLine($"PlayerId : {entity.Id}, Speed : {entity.Speed}");
        }
    }
    
    public void Add(Entity entity)
    {
        _turnOrderBySpeed.Add(entity);
        Sort();
    }

    public void Remove(Entity entity)
    {
        int index = _turnOrderBySpeed.IndexOf(entity);
        
        if (index < _order) 
            _order--;

        _turnOrderBySpeed.RemoveAt(index);
        Sort();
    }
}