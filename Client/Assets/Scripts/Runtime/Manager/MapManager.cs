using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    private List<Cube> _cubeList = new List<Cube>();
    public IReadOnlyList<Cube> CubeList => _cubeList;
    public GameObject Map { get; set; }

    public void Init()
    {
        GameObject map = new GameObject();
        map.name = "Map";
        map.transform.position = new Vector3(0, 0, 0);
        Map = map;
    }
    
    public void GenerateCube(Vector3 position, bool data)
    {
        if (!data)
            return;
        
        Object obj = Resources.Load("Cube");
        GameObject go = Object.Instantiate(obj, Map.transform) as GameObject;
        Cube cube = go.GetOrAddComponent<Cube>();

        cube.transform.position = new Vector3(position.x, position.y - 1, position.z);
        cube.name = $"{cube.transform.position}";
        _cubeList.Add(cube);
    }
}