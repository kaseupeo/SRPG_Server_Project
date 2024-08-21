namespace Server;

public class MapManager
{
    public static MapManager Instance { get; } = new MapManager();

    public int Width { get; set; } = 10;
    public int Length { get; set; } = 10;

    private Dictionary<(int, int, int), bool> _map = new Dictionary<(int, int, int), bool>();
    public IReadOnlyDictionary<(int, int, int), bool> Map => _map;

    public Dictionary<(int, int, int), Entity> EntitiesOnMapDic { get; set; } =
        new Dictionary<(int, int, int), Entity>();
    
    public void GenerateMap()
    {
        Random random = new Random();
        Dictionary<(int, int, int), bool> map = new Dictionary<(int, int, int), bool>();

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Length; z++)
            {
                // 0이면 true (이동 가능), 1이면 false (장애물)
                map[(x, 0, z)] = (random.Next(10) != 0);
            }
        }
        
        _map = map;
    }

    public void UpdateMap()
    {
        
    }
}