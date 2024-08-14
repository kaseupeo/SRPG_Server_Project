using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager
{
    private Entity _player;
    private Dictionary<int, Entity> _otherPlayerDic = new Dictionary<int, Entity>();
    
    public void Init()
    {
        
    }
    
    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerID == _player.ID)
            return;

        Object obj = Resources.Load("Player");
        GameObject go  =Object.Instantiate(obj) as GameObject;

        Entity entity = go.AddComponent<Entity>();
        entity.transform.position = new Vector3(packet.X, packet.Y, packet.Z);
        _otherPlayerDic.Add(packet.playerID, entity);
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (_player.ID == packet.playerID)
        {
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            if (_otherPlayerDic.TryGetValue(packet.playerID, out var playerController))
            {
                GameObject.Destroy(playerController.gameObject);
                _otherPlayerDic.Remove(packet.playerID);
            }
        }
    }
    
    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (S_PlayerList.Player p in packet.playerList)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            Entity entity = go.AddComponent<Entity>();
            entity.ID = p.playerID;
            entity.transform.position = new Vector3(p.X, p.Y, p.Z);

            if (p.isSelf)
            {
                go.AddComponent<PlayerController>();
                entity.GetComponent<MeshRenderer>().material.color = Color.blue;
                _player = entity;
            }
            else
            {
                _otherPlayerDic.Add(p.playerID, entity);
            }
        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if (_player.ID == packet.playerID)
            _player.transform.position = new Vector3(packet.X, packet.Y, packet.Z);
        else if (_otherPlayerDic.TryGetValue(packet.playerID, out var playerController))
                playerController.transform.position = new Vector3(packet.X, packet.Y, packet.Z);
    }

    public void FindValidPosition(S_ValidPosition packet)
    {
        List<Cube> cubeList = GameObject.Find("MapGenerate").GetComponent<GenerateTileMap>().CubeList;

        foreach (Cube cube in cubeList) 
            cube.MoveTile.SetActive(false);

        foreach (S_ValidPosition.Position position in packet.positionList)
        {
            Debug.Log($"({position.X}, {position.Z})");
        }

        var join = cubeList.Join(packet.positionList,
            cube => new Vector2Int((int)cube.transform.position.x, (int)cube.transform.position.z),
            move => new Vector2Int((int)move.X, (int)move.Z),
            (cube, move) => cube);

        foreach (Cube cube in join) 
            cube.MoveTile.SetActive(true);
    }
    
    public void Move2(S_Move packet)
    {
        if (_player.ID == packet.playerID)
            _player.transform.position = new Vector3(packet.pathList[0].X, packet.pathList[0].Y, packet.pathList[0].Z);
        else if (_otherPlayerDic.TryGetValue(packet.playerID, out var playerController))
            playerController.transform.position = new Vector3(packet.pathList[0].X, packet.pathList[0].Y, packet.pathList[0].Z);
    }
    
    public void Clear()
    {
        _player = null;
        _otherPlayerDic.Clear();
    }
    
    public void GameQuit()
    {
        Managers.Clear();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}