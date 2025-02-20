using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    //ī�޶� �����ϱ�
    private GameManager gameManager;
    private Camera camera;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        camera = Camera.main;
    }
    //�÷��̾� �ൿ �޾Ƴ��� �޼���
    protected override void HandleAction()
    {
        //Axis �� ���� �̿��ؼ� Ű �Է� �ޱ� 
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //���� ���ͷ� �̿��ϱ� ���� �� �ʱ�ȭ
        movementDirection = new Vector2(horizontal, vertical).normalized;
        
        //���콺 ��ġ ���ϱ�
        //���콺�� ��ǥ�� �ػ��� ��ǥ, �� ��ǥ�� ���� �� ��ǥ�� �ٲ��ֱ�
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        //���� ���� ĳ������ ��ġ�� ���� ���� ������ �ٲٱ�
        lookDirection = (worldPos - (Vector2)transform.position);

        if(lookDirection.magnitude<.9f) //�ٶ� ��ġ�� �Ÿ��� 0.9���� �� �Ǹ�
        {
            lookDirection = Vector2.zero; //�ٶ󺸴� ��ġ ����
        }
        else //�ƴ϶��
        {
            lookDirection = lookDirection.normalized; //���콺 ��ġ�� ���� ���ͷ� ���� ���� �ʱ�ȭ���ֱ�
        }

        isAttacking = Input.GetMouseButton(0);
    }

    public override void Death()
    {
        base.Death();
        gameManager.GameOver();
    }

}
