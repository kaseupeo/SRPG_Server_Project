namespace Server;

public class PathFind
{
    private HashSet<(int, int, int)> _tileHashSet = new HashSet<(int, int, int)>();

    private readonly (int x, int y, int z)[] _directions =
    [
        (1, 0, 0), // 오른쪽
        (-1, 0, 0), // 왼쪽
        (0, 0, 1), // 위쪽
        (0, 0, -1) // 아래쪽
    ];
    
    public HashSet<(int, int, int)> FindTile(Dictionary<(int, int, int), bool> map, (int x, int y, int z) currentPos, int moveRange)
    {
        _tileHashSet.Add(currentPos);

        if (moveRange > 0)
        {
            foreach (var direction in _directions)
            {
                var newPos = (currentPos.x + direction.x, currentPos.y + direction.y, currentPos.z + direction.z);

                if (map.ContainsKey(newPos) && map[newPos])
                    FindTile(map, newPos, moveRange - 1);
            }
        }

        return _tileHashSet;
    }
}