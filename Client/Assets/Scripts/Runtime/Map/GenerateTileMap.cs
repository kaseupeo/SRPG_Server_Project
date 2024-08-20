using System.Collections.Generic;
using UnityEngine;

public class GenerateTileMap : MonoBehaviour
{
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private Vector2 mapScale;

    // public List<Cube> CubeList = new List<Cube>();

    public void Generate(Vector3 position)
    {
        Cube go = Instantiate(cubePrefab, transform, true);
        go.gameObject.name = $"({position.x}, {position.y}, {position.z})";
        go.transform.position = position;
    }
    
// #if UNITY_EDITOR
//     [ContextMenu("Generate Cube Tile")]
//     private void Generate()
//     {
//         for (int i = 0; i < mapScale.x; i++)
//         {
//             for (int j = 0; j < mapScale.y; j++)
//             {
//                 Cube go = Instantiate(cubePrefab, transform, true);
//                 go.gameObject.name = $"({i}, {j})";
//                 go.transform.position = new Vector3(i, transform.position.y, j);
//                 go.GetComponent<MeshRenderer>().material.color = (i + j) % 2 != 0 ? Color.white : Color.black;
//                 CubeList.Add(go);
//             }
//         }
//     }
//
//     [ContextMenu("Clear Cube Tile")]
//     private void Clear()
//     {
//         CubeList.Clear();
//         
//         foreach (Cube cube in gameObject.GetComponentsInChildren<Cube>())
//         {
//             DestroyImmediate(cube.gameObject);
//         }
//     }
// #endif
}
