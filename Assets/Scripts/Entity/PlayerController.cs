using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    //카메라 설정하기
    private GameManager gameManager;
    private Camera camera;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        camera = Camera.main;
    }
    //플레이어 행동 받아놓는 메서드
    protected override void HandleAction()
    {
        //Axis 축 값을 이용해서 키 입력 받기 
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //방향 벡터로 이용하기 위해 값 초기화
        movementDirection = new Vector2(horizontal, vertical).normalized;
        
        //마우스 위치 구하기
        //마우스의 좌표는 해상도의 좌표, 그 좌표를 게임 내 좌표로 바꿔주기
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        //이후 현재 캐릭터의 위치를 빼서 로컬 값으로 바꾸기
        lookDirection = (worldPos - (Vector2)transform.position);

        if(lookDirection.magnitude<.9f) //바라본 위치의 거리가 0.9조차 안 되면
        {
            lookDirection = Vector2.zero; //바라보는 위치 없음
        }
        else //아니라면
        {
            lookDirection = lookDirection.normalized; //마우스 위치를 방향 벡터로 쓰기 위해 초기화해주기
        }

        isAttacking = Input.GetMouseButton(0);
    }

    public override void Death()
    {
        base.Death();
        gameManager.GameOver();
    }

}
