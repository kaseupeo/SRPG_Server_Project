using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManager
{
    private PlayerController _player;
    private Dictionary<int, Entity> _entityDic = new Dictionary<int, Entity>();

    public PlayerController Player => _player;
    public bool IsStartGame { get; set; } = false;

    public void Init()
    {
        
    }
    
    // 내가 접속하기전부터 있던 플레이어 오브젝트 생성 및 내 오브젝트 생성
    public void EnterGame(S_PlayerList packet)
    {
        // Object obj = Resources.Load("Entity");

        foreach (S_PlayerList.Player p in packet.playerList)
        {
            Entity go = CreateEntityObject(p.EntityId);
            // GameObject go = Object.Instantiate(obj) as GameObject;
            // Entity entity = go.GetOrAddComponent<Entity>();
            //
            // entity.Id = p.EntityId;
            // go.name = $"{p.EntityId}";
            // _entityDic.Add(p.EntityId, entity);
            
            if (p.IsSelf)
            {
                _player = go.gameObject.AddComponent<PlayerController>();
                // _player.Init();

                Debug.Log($"Enter Game : {p.EntityId}");
            }

            Debug.Log($"Connected : {p.EntityId} ");
        }
    }
    
    // 내가 접속 중 새로 들어온 플레이어 생성
    public void EnterGame(S_EnterGame packet)
    {
        if (packet.EntityId == _player.Entity.Id)
            return;

        CreateEntityObject(packet.EntityId);
        // Object obj = Resources.Load("Entity");
        // GameObject go = Object.Instantiate(obj) as GameObject;
        // Entity entity = go.GetOrAddComponent<Entity>();
        //
        // go.name = $"{packet.EntityId}";
        // entity.Id = packet.EntityId;
        // _entityDic.Add(packet.EntityId, entity);

        Debug.Log($"Enter Game : {packet.EntityId}");
    }
    
    public void LeaveGame(S_LeaveGame packet)
    {
        if (_player.Entity.Id == packet.EntityId)
        {
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            if (_entityDic.TryGetValue(packet.EntityId, out var entity))
            {
                GameObject.Destroy(entity.gameObject);
                _entityDic.Remove(packet.EntityId);
            }
        }
    }

    public void GenerateMap(S_MapData packet)
    {
        foreach (S_MapData.Map map in packet.mapList)
        {
            Managers.Map.GenerateCube(new Vector3(map.X, map.Y, map.Z), map.Data);
        }
    }
    
    public void ReadyGame(S_ReadyGame packet)
    {
        foreach (S_ReadyGame.Player player in packet.playerList)
        {
            // TODO : 준비 표시 UI 변경
            Debug.Log($"{player.EntityId} : {player.IsReady}");
        }
    }

    public void StartGame(S_StartGame packet)
    {
        Debug.Log("Start Game");
        // TODO : 게임 시작
        foreach (S_StartGame.Entity e in packet.entityList)
        {
            if (!_entityDic.TryGetValue(e.EntityId, out var entity))
            {
                entity = CreateEntityObject(e.EntityId);
            }

            CreateEntityModelObject(entity);
            entity.transform.position = new Vector3(e.X, e.Y, e.Z);
            
            if (_player.Entity == entity)
                entity.Model.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        
        IsStartGame = true;
        // TODO : 게임 시작 알림같은거? 추가?

    }

    public void StartTurn(S_StartTurn packet)
    {
        // TODO : 턴 시작
        Debug.Log($"Start Turn : {packet.EntityId}");
    }

    public void ShowRange(S_ActionRange packet)
    {
        // List<Cube> list = Managers.Map.CubeList.ToList();
        IReadOnlyList<Cube> list = Managers.Map.CubeList;

        foreach (Cube cube in list)
        {
            cube.MoveTile.SetActive(false);
            cube.AttackTile.SetActive(false);
        }
        
        if (packet.EntityId != _player.Entity.Id)
            return;
        
        var join = list.Join(packet.positionList,
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
        if (_entityDic.TryGetValue(packet.EntityId, out var entity))
        {
            List<Vector3> position = new List<Vector3>();

            foreach (S_Move.Path path in packet.pathList) 
                position.Add(new Vector3(path.X, path.Y, path.Z));
            
            entity.Move(position);
        }
    }
    
    public void Attack(S_Attack packet)
    {
        // 공격 수행자 체크 & 공격 피격자 체크
        if (_entityDic.TryGetValue(packet.EntityId, out var entity) && _entityDic.TryGetValue(packet.TargetId, out var target))
        {
            entity.Attack(packet.Damage);
            target.TakeDamage(packet.MaxHp, packet.Hp, packet.Damage);
            entity.State = Define.EntityState.EndTurn;
        }
        
        Debug.Log($"player ID : {packet.EntityId} target ID : {packet.TargetId} target Hp : {packet.Hp}, Damage : {packet.Damage}");
    }

    public void Dead(S_Dead packet)
    {
        _entityDic.Remove(packet.EntityId, out var entity);
        entity.Dead();
    }
    
    private Entity CreateEntityObject(int id)
    {
        GameObject go = Object.Instantiate(new GameObject()) as GameObject;
        Entity entity = go.GetOrAddComponent<Entity>();
        entity.Id = id;
        go.name = $"{id}";
        _entityDic.Add(id, entity);

        return entity;
    }

    private void CreateEntityModelObject(Entity entity)
    {
        Object model = Resources.Load("Player Model");
        Object hpBar = Resources.Load("Hp");
        GameObject modelGameObject = GameObject.Instantiate(model, entity.transform, true) as GameObject;
        GameObject hpBarCanvas = GameObject.Instantiate(hpBar, entity.transform, true) as GameObject;
        UIHpBar uiHpBar = hpBarCanvas.GetComponentInChildren<UIHpBar>();

        entity.Model = modelGameObject;
        entity.HpChanged += uiHpBar.Change;
    }
    
    public void Clear()
    {
        _player = null;
        _entityDic.Clear();
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