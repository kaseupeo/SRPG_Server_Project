using System;
using UnityEngine;

public class Managers : MonoBehaviour
{
    #region 싱글톤

    private static Managers _instance;
    private static bool _isQuitting;

    public static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    private static void Init()
    {
        if (_isQuitting || _instance != null)
            return;

        GameObject go = GameObject.Find("Managers");
        if (go == null)
            go = new GameObject("Managers");

        DontDestroyOnLoad(go);
        _instance = go.GetComponent<Managers>();
    }

    #endregion

    private NetworkManager _network = new();
    private GameManager _game = new();
    private MapManager _map = new();

    public static NetworkManager Network => Instance?._network;
    public static GameManager Game => Instance?._game;
    public static MapManager Map => Instance?._map;
    
    private void Awake()
    {
        Network.Init();
        Map.Init();
    }

    private void Update()
    {
        Network.Update();
    }

    private void OnApplicationQuit()
    {
        Clear();
        _isQuitting = true;
    }

    public static void Clear()
    {
        
    }
}