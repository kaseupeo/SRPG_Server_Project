namespace Server;

public class AStar
{
    private IReadOnlyDictionary<(int, int, int), bool> _map;
    private List<Node> _openList;
    private HashSet<(int x, int y, int z)> _closedList;

    public AStar(IReadOnlyDictionary<(int, int, int), bool> map)
    {
        _map = map;
        _openList = new List<Node>();
        _closedList = new HashSet<(int x, int y, int z)>();
    }

    public List<(int x, int y, int z)> FindPath((int x, int y, int z) start, (int x, int y, int z) goal)
    {
        _openList.Clear();
        _closedList.Clear();

        Node startNode = new Node(start, null, 0, GetHeuristic(start, goal));
        _openList.Add(startNode);

        while (_openList.Count > 0)
        {
            Node currentNode = _openList[0];
            foreach (var node in _openList)
            {
                if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                {
                    currentNode = node;
                }
            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode.Position);

            if (currentNode.Position == goal)
            {
                return RetracePath(currentNode);
            }

            foreach (var neighbor in GetNeighbors(currentNode.Position))
            {
                if (_closedList.Contains(neighbor) || !_map[neighbor])
                {
                    continue;
                }

                int tentativeG = currentNode.G + GetDistance(currentNode.Position, neighbor);
                Node neighborNode = _openList.Find(n => n.Position == neighbor);

                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbor, currentNode, tentativeG, GetHeuristic(neighbor, goal));
                    _openList.Add(neighborNode);
                }
                else if (tentativeG < neighborNode.G)
                {
                    neighborNode.G = tentativeG;
                    neighborNode.Parent = currentNode;
                }
            }
        }

        return null; // 경로를 찾지 못한 경우
    }

    private int GetHeuristic((int x, int y, int z) a, (int x, int y, int z) b)
    {
        // 맨해튼 거리 사용
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
    }

    private int GetDistance((int x, int y, int z) a, (int x, int y, int z) b)
    {
        // 가로, 세로 또는 높이 한 칸 이동은 거리 1
        return 1;
    }

    private List<(int x, int y, int z)> GetNeighbors((int x, int y, int z) position)
    {
        List<(int x, int y, int z)> neighbors = new List<(int x, int y, int z)>();

        // 대각선 이동을 제외한 6방향 탐색
        int[][] directions =
        [
            [1, 0, 0], // +X 방향
            [-1, 0, 0], // -X 방향
            [0, 1, 0], // +Y 방향
            [0, -1, 0], // -Y 방향
            [0, 0, 1], // +Z 방향
            [0, 0, -1] // -Z 방향
        ];

        foreach (var direction in directions)
        {
            var neighbor = (position.x + direction[0], position.y + direction[1], position.z + direction[2]);
            if (_map.ContainsKey(neighbor) && _map[neighbor]) 
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private List<(int x, int y, int z)> RetracePath(Node endNode)
    {
        List<(int x, int y, int z)> path = new List<(int x, int y, int z)>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private class Node
    {
        public (int x, int y, int z) Position { get; }
        public Node Parent { get; set; }
        public int G { get; set; }
        public int H { get; }
        public int F => G + H;

        public Node((int x, int y, int z) position, Node parent, int g, int h)
        {
            Position = position;
            Parent = parent;
            G = g;
            H = h;
        }
    }
}