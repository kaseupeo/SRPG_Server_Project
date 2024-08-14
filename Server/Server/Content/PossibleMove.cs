public class PossibleMove
{
    private HashSet<(int, int)> _tileHashSet = new HashSet<(int, int)>();
    public HashSet<(int, int)> FindTile(Dictionary<(int, int), bool> map, (int x, int y) currentPos, int moveRange)
    {
        var directions = new (int x, int y)[]
        {
            (1, 0),  // 오른쪽
            (-1, 0), // 왼쪽
            (0, 1),  // 위쪽
            (0, -1)  // 아래쪽
        };

        _tileHashSet.Add(currentPos);

        if (moveRange > 0)
        {
            foreach (var direction in directions)
            {
                var newPos = (currentPos.x + direction.x, currentPos.y + direction.y);

                if (map.ContainsKey(newPos) && map[newPos]) 
                    FindTile(map, newPos, moveRange - 1);
            }
        }

        return _tileHashSet;
    }
}