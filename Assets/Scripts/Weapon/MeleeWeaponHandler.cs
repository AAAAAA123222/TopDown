using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeWeaponHandler : WeaponHandler
{
    [Header("Melee Attack Info")]
    public Vector2 collideBoxSize = Vector2.one;

    protected override void Start()
    {
        //충돌 판정을 무기 크기와 맞추기
        base.Start();
        collideBoxSize = collideBoxSize * WeaponSize;
    }

    public override void Attack()
    {
        base.Attack();

        //형체를 가진 레이캐스트
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position + (Vector3)Controller.LookDirection * collideBoxSize.x, //시전자의 위치 + 보고 있는 방향을 기점으로 무기의 x값만큼 떨어진 거리에서
            collideBoxSize, //무기 크기만큼 캐스팅
            0, //각도는 0 (이미 어디로 뻗어나갈지 위치가 잡혔으니까)
            Vector2.zero, //방향 역시 0 (마찬가지로 이미 위치가 있으니)
            0, //0
            target //target 레이어를 탐색하기
            );

        if (hit.collider != null) //타겟과 충돌했다면
        {

            ResourceController resourceController = hit.collider.GetComponent<ResourceController>();
            if (resourceController != null) //컨트롤러가 있다면(깎을 체력이 있다면)
            {
                resourceController.ChangeHealth(-Power); //체력 깎기
                if (IsOnKnockback)
                {
                    BaseController controller = hit.collider.GetComponent<BaseController>();
                    if (controller != null) //넉백도 있다면 
                    {
                        //넉백 가하기
                        controller.ApplyKnockback(transform, KnockbackPower, KnockbackTime);
                    }
                }
            }
        }
    }

    public override void Rotate(bool isLeft)
    {
        if (isLeft)
            transform.eulerAngles = new Vector3(0, 180, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
