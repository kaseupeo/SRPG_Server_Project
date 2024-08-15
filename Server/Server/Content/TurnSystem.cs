namespace Server;

public class TurnSystem
{
    private List<EntityData> _entityList = new List<EntityData>();
    public Queue<EntityData> EntityQueue { get; set; } = new Queue<EntityData>();

    public void Add(EntityData entityData)
    {
        _entityList.Add(entityData);
    }
    
    public void Sort()
    {
        _entityList.Sort((x, y) => y.Speed.CompareTo(x.Speed));

        EntityQueue.Clear();
        
        Console.WriteLine("순서");
        foreach (EntityData entity in _entityList)
        {
            EntityQueue.Enqueue(entity);
            Console.WriteLine($"PlayerID : {entity.Id}, Speed : {entity.Speed}");
        }
    }
}