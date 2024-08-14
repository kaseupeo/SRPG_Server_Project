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
            C_StartGame startGamePacket = new C_StartGame();
            startGamePacket.X = transform.position.x;
            startGamePacket.Y = 0;
            startGamePacket.Z = transform.position.z;
            Managers.Network.Send(startGamePacket.Write());
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                string pos = hit.collider.gameObject.name;
                Vector3 pos2 = hit.collider.gameObject.transform.position;

                // Debug.Log($"name : {pos}, pos : {pos2}");
                
                // TODO : 이동할 수 있는 위치 중에서 선택 후 보내기
                C_ClickPosition movePacket = new C_ClickPosition();
                movePacket.X = pos2.x;
                movePacket.Z = pos2.z;
                Managers.Network.Send(movePacket.Write());
            }
        }
    }

    // private void FixedUpdate()
    // {
    //     float moveX = Input.GetAxis("Horizontal");
    //     float moveZ = Input.GetAxis("Vertical");
    //
    //     C_Move movePacket = new C_Move();
    //
    //     movePacket.X += moveX * 10 * Time.fixedDeltaTime + transform.position.x;
    //     movePacket.Z += moveZ * 10 * Time.fixedDeltaTime + transform.position.z;
    //     
    //     Managers.Network.Send(movePacket.Write());
    // }
}
