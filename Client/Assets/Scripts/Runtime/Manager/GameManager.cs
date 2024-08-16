using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManager
{
    private Entity _player;
    private Dictionary<int, Entity> _playerDic = new Dictionary<int, Entity>();

    public bool IsStartGame { get; set; } = false;

    public void Init()
    {
        
    }
    
    // 내가 접속하기전부터 있던 플레이어 오브젝트 생성 및 내 오브젝트 생성
    public void EnterGame(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (S_PlayerList.Player p in packet.playerList)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            Entity entity = go.GetOrAddComponent<Entity>();
            
            entity.ID = p.PlayerId;
            go.name = $"{p.PlayerId}";
            _playerDic.Add(p.PlayerId, entity);
            
            if (p.IsSelf)
            {
                go.AddComponent<PlayerController>();
                _player = entity;

                Debug.Log($"Enter Game : {p.PlayerId}");
            }

            Debug.Log($"Connected : {p.PlayerId} ");
        }
    }
    
    // 내가 접속 중 새로 들어온 플레이어 생성
    public void EnterGame(S_EnterGame packet)
    {
        if (packet.PlayerId == _player.ID)
            return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;
        Entity entity = go.GetOrAddComponent<Entity>();

        go.name = $"{packet.PlayerId}";
        entity.ID = packet.PlayerId;
        _playerDic.Add(packet.PlayerId, entity);

        Debug.Log($"Enter Game : {packet.PlayerId}");
    }

    public void LeaveGame(S_LeaveGame packet)
    {
        if (_player.ID == packet.PlayerId)
        {
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            if (_playerDic.TryGetValue(packet.PlayerId, out var entity))
            {
                GameObject.Destroy(entity.gameObject);
                _playerDic.Remove(packet.PlayerId);
            }
        }
    }

    public void ReadyGame(S_ReadyGame packet)
    {
        foreach (S_ReadyGame.Player player in packet.playerList)
        {
            // TODO : 준비 표시 UI 변경
            Debug.Log($"{player.PlayerId} : {player.IsReady}");
        }
    }

    public void StartGame(S_StartGame packet)
    {
        // TODO : 게임 시작
        foreach (S_StartGame.Entity entity in packet.entityList)
        {
            if (_playerDic.TryGetValue(entity.PlayerId, out var player))
            {
                // TODO : 나중에 Entity 메소드로 빼기
                Object obj = Resources.Load("Player Model");
                GameObject go = GameObject.Instantiate(obj, player.transform, true) as GameObject;

                player.Model = go;
                player.transform.position = new Vector3(entity.X, entity.Y, entity.Z);
                if (_player == player)
                    player.Model.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }
        
        IsStartGame = true;
        // TODO : 게임 시작 알림같은거? 추가?

    }

    public void StartTurn(S_StartTurn packet)
    {
        // TODO : 턴 시작
    }

    public void ShowMoveRange(S_MoveRange packet)
    {
        List<Cube> cubeList = GameObject.Find("MapGenerate").GetComponent<GenerateTileMap>().CubeList;

        foreach (Cube cube in cubeList) 
            cube.MoveTile.SetActive(false);
        
        if (packet.PlayerId != _player.ID)
            return;
        
        var join = cubeList.Join(packet.positionList,
            cube => new Vector2Int((int)cube.transform.position.x, (int)cube.transform.position.z),
            move => new Vector2Int((int)move.X, (int)move.Z),
            (cube, move) => cube);

        foreach (Cube cube in join) 
            cube.MoveTile.SetActive(true);
    }
    
    public void Move(S_Move packet)
    {
        // TODO : 임시 
        if (_player.ID == packet.PlayerId)
            _player.transform.position = new Vector3(packet.pathList[0].X, packet.pathList[0].Y, packet.pathList[0].Z);
        else if (_playerDic.TryGetValue(packet.PlayerId, out var playerController))
            playerController.transform.position = new Vector3(packet.pathList[0].X, packet.pathList[0].Y, packet.pathList[0].Z);
    }
    
    public void Clear()
    {
        _player = null;
        _playerDic.Clear();
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