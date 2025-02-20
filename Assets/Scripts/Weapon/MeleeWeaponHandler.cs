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
        //�浹 ������ ���� ũ��� ���߱�
        base.Start();
        collideBoxSize = collideBoxSize * WeaponSize;
    }

    public override void Attack()
    {
        base.Attack();

        //��ü�� ���� ����ĳ��Ʈ
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position + (Vector3)Controller.LookDirection * collideBoxSize.x, //�������� ��ġ + ���� �ִ� ������ �������� ������ x����ŭ ������ �Ÿ�����
            collideBoxSize, //���� ũ�⸸ŭ ĳ����
            0, //������ 0 (�̹� ���� ������� ��ġ�� �������ϱ�)
            Vector2.zero, //���� ���� 0 (���������� �̹� ��ġ�� ������)
            0, //0
            target //target ���̾ Ž���ϱ�
            );

        if (hit.collider != null) //Ÿ�ٰ� �浹�ߴٸ�
        {

            ResourceController resourceController = hit.collider.GetComponent<ResourceController>();
            if (resourceController != null) //��Ʈ�ѷ��� �ִٸ�(���� ü���� �ִٸ�)
            {
                resourceController.ChangeHealth(-Power); //ü�� ���
                if (IsOnKnockback)
                {
                    BaseController controller = hit.collider.GetComponent<BaseController>();
                    if (controller != null) //�˹鵵 �ִٸ� 
                    {
                        //�˹� ���ϱ�
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
