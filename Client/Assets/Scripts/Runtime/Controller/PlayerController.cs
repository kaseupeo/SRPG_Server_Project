using System;
using UnityEngine;
using State = Define.EntityState;

public class PlayerController : BaseController
{
    
    public override void Init()
    {
        base.Init();
    }

    private void Update()
    {
        // 게임 준비 
        if (!Managers.Game.IsStartGame)
            return;

        if ((Entity.State & State.ShowRange) == State.ShowRange) 
            ShowRange();
        
        if ((Entity.State & State.Move) == State.Move) 
            Move();
        
        if ((Entity.State & State.Attack) == State.Attack) 
            Attack();

        // 턴 종료
        if (Entity.State == State.EndTurn) 
            EndTurn();
    }

    public void ChangeState(Define.EntityState state)
    {
        Entity.State = state;
    }
    
    public void ReadyGame()
    {
        C_ReadyGame readyPacket = new C_ReadyGame();
        readyPacket.IsReady = true;
        Managers.Network.Send(readyPacket.Write());
    }

    public void EndTurn()
    {
        C_EndTurn endTurn = new C_EndTurn();
        Managers.Network.Send(endTurn.Write());
        Entity.State = State.Waiting;
        
    }
    
    public void ShowRange()
    {
        C_PlayerState statePacket = new C_PlayerState();
        statePacket.State = (int)Entity.State;
        statePacket.X = transform.position.x;
        statePacket.Y = 0;
        statePacket.Z = transform.position.z;
        Managers.Network.Send(statePacket.Write());
    }
    
    public void Move()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            Vector3 position = hit.collider.gameObject.transform.position;
                
            // TODO : 이동할 수 있는 위치 중에서 선택 후 보내기
            C_PlayerState statePacket = new C_PlayerState();
            statePacket.State = (int)Entity.State;
            statePacket.X = position.x;
            statePacket.Z = position.z;
            Managers.Network.Send(statePacket.Write());
        }
    }
    
    public void Attack()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            Vector3 position = hit.collider.gameObject.transform.position;
                
            C_PlayerState statePacket = new C_PlayerState();
            statePacket.State = (int)Entity.State;
            statePacket.X = position.x;
            statePacket.Z = position.z;
            Managers.Network.Send(statePacket.Write());
        }
    }
}
