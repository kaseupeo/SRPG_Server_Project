using System;
using UnityEngine;

public class PlayerController : BaseController
{
    protected override void Init()
    {
        base.Init();
    }

    private void Update()
    {
        // 게임 준비 
        if (!Managers.Game.IsStartGame)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                C_ReadyGame readyPacket = new C_ReadyGame();
                readyPacket.IsReady = true;
                Managers.Network.Send(readyPacket.Write());
            }
            
            return;
        }
        
        // 이동 범위
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            C_PlayerState statePacket = new C_PlayerState();
            statePacket.State = 0;
            statePacket.X = transform.position.x;
            statePacket.Y = 0;
            statePacket.Z = transform.position.z;
            Managers.Network.Send(statePacket.Write());
        }
        
        // 이동
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Vector3 position = hit.collider.gameObject.transform.position;
                
                // TODO : 이동할 수 있는 위치 중에서 선택 후 보내기
                C_PlayerState statePacket = new C_PlayerState();
                statePacket.State = 1;
                statePacket.X = position.x;
                statePacket.Z = position.z;
                Managers.Network.Send(statePacket.Write());
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            C_PlayerState statePacket = new C_PlayerState();
            statePacket.State = 2;
            statePacket.X = transform.position.x;
            statePacket.Y = 0;
            statePacket.Z = transform.position.z;
            Managers.Network.Send(statePacket.Write());
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Vector3 position = hit.collider.gameObject.transform.position;
                
                C_PlayerState statePacket = new C_PlayerState();
                statePacket.State = 3;
                statePacket.X = position.x;
                statePacket.Z = position.z;
                Managers.Network.Send(statePacket.Write());
            }
        }
        
        // 턴 종료
        if (Input.GetKeyDown(KeyCode.Space))
        {
            C_EndTurn endTurn = new C_EndTurn();
            Managers.Network.Send(endTurn.Write());
        }
    }
}
