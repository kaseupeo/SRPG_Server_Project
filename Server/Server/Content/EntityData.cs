namespace Server;

public class EntityData(int id)
{
    // Entity ID
    public int Id { get; set; } = id;

    // Entity Position
    public (int X, int Y, int Z) Position { get; set; }
    
    // Entity Valid Position
    public HashSet<(int, int, int)> ValidPosition { get; set; } = new HashSet<(int, int, int)>();

    // TODO : Entity Stat
    public int Speed { get; set; }
}