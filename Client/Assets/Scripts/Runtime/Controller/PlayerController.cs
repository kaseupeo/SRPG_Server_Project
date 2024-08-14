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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            C_PlayerAction actionPacket = new C_PlayerAction();
            actionPacket.action = 0;
            actionPacket.X = transform.position.x;
            actionPacket.Y = 0;
            actionPacket.Z = transform.position.z;
            Managers.Network.Send(actionPacket.Write());
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                string pos = hit.collider.gameObject.name;
                Vector3 pos2 = hit.collider.gameObject.transform.position;

                // Debug.Log($"name : {pos}, pos : {pos2}");
                
                // TODO : 이동할 수 있는 위치 중에서 선택 후 보내기
                C_PlayerAction actionPacket = new C_PlayerAction();
                actionPacket.action = 1;
                actionPacket.X = pos2.x;
                actionPacket.Z = pos2.z;
                Managers.Network.Send(actionPacket.Write());
            }
        }
    }
}
