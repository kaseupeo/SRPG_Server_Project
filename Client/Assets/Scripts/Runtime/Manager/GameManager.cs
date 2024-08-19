using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManager
{
    private PlayerController _player;
    private Dictionary<int, Entity> _playerDic = new Dictionary<int, Entity>();

    public PlayerController Player => _player;
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
            
            entity.Id = p.PlayerId;
            go.name = $"{p.PlayerId}";
            _playerDic.Add(p.PlayerId, entity);
            
            if (p.IsSelf)
            {
                _player = go.AddComponent<PlayerController>();
                _player.Init();

                Debug.Log($"Enter Game : {p.PlayerId}");
            }

            Debug.Log($"Connected : {p.PlayerId} ");
        }
    }
    
    // 내가 접속 중 새로 들어온 플레이어 생성
    public void EnterGame(S_EnterGame packet)
    {
        if (packet.PlayerId == _player.Entity.Id)
            return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;
        Entity entity = go.GetOrAddComponent<Entity>();

        go.name = $"{packet.PlayerId}";
        entity.Id = packet.PlayerId;
        _playerDic.Add(packet.PlayerId, entity);

        Debug.Log($"Enter Game : {packet.PlayerId}");
    }

    public void LeaveGame(S_LeaveGame packet)
    {
        if (_player.Entity.Id == packet.PlayerId)
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
                if (_player.Entity == player)
                    player.Model.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }
        
        IsStartGame = true;
        // TODO : 게임 시작 알림같은거? 추가?

    }

    public void StartTurn(S_StartTurn packet)
    {
        // TODO : 턴 시작
        Debug.Log($"Start Turn : {packet.PlayerId}");
    }

    public void ShowRange(S_ActionRange packet)
    {
        List<Cube> cubeList = GameObject.Find("MapGenerate").GetComponent<GenerateTileMap>().CubeList;

        foreach (Cube cube in cubeList)
        {
            cube.MoveTile.SetActive(false);
            cube.AttackTile.SetActive(false);
        }
        
        if (packet.PlayerId != _player.Entity.Id)
            return;
        
        var join = cubeList.Join(packet.positionList,
            cube => new Vector2Int((int)cube.transform.position.x, (int)cube.transform.position.z),
            move => new Vector2Int((int)move.X, (int)move.Z),
            (cube, move) => cube);

        
        foreach (Cube cube in join)
        {
            if (Define.EntityState.Move == (_player.Entity.State & Define.EntityState.Move))
                cube.MoveTile.SetActive(true);
            else if (Define.EntityState.Attack == (_player.Entity.State & Define.EntityState.Attack))
                cube.AttackTile.SetActive(true);
        }

        Debug.Log($"상태 : {_player.Entity.State}");
        _player.Entity.State &= ~Define.EntityState.ShowRange;
    }
    
    public void Move(S_Move packet)
    {
        if (_playerDic.TryGetValue(packet.PlayerId, out var entity))
        {
            List<Vector3> position = new List<Vector3>();

            foreach (S_Move.Path path in packet.pathList) 
                position.Add(new Vector3(path.X, path.Y, path.Z));
            
            entity.Move(position);
        }
    }
    
    public void Attack(S_Attack packet)
    {
        if (_playerDic.TryGetValue(packet.PlayerId, out var entity) && _playerDic.TryGetValue(packet.TargetId, out var target))
        {
            entity.Attack(packet.Damage);
            target.TakeDamage(packet.Hp, packet.Damage);
            entity.State = Define.EntityState.EndTurn;
        }
        
        Debug.Log($"player ID : {packet.PlayerId} target ID : {packet.TargetId} target Hp : {packet.Hp}, Damage : {packet.Damage}");
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