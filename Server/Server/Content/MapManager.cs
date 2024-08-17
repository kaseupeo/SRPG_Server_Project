namespace Server;

public class MapManager
{
    public static MapManager Instance { get; } = new MapManager();

    public Dictionary<(int, int, int), bool> Map { get; set; } = new Dictionary<(int, int, int), bool>();

    public Dictionary<(int, int, int), Entity> EntitiesOnMapDic { get; set; } =
        new Dictionary<(int, int, int), Entity>();
    
    public void GenerateMap()
    {
        int[,] grid = new int[10, 10]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        Dictionary<(int, int, int), bool> map = new Dictionary<(int, int, int), bool>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                // 0이면 true (이동 가능), 1이면 false (장애물)
                map[(x, 0, z)] = grid[x, z] == 0;
            }
        }
        
        Map = map;
    }

    public void UpdateMap()
    {
        
    }
}