public class PlayerData
{
    // Player Position
    public (int X, int Y, int Z) Position { get; set; }
    
    // Player Valid Position
    public HashSet<(int, int, int)> ValidPosition { get; set; } = new HashSet<(int, int, int)>();

    // TODO : Player Stat
    
}