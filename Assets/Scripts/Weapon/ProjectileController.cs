using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask levelCollisionLayer;
    private RangeWeaponHandler rangeWeaponHandler;

    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;

    ProjectileManager projectileManager;

    //������ �� ����Ʈ ����ϱ�
    public bool fxOnDestroy = true;

    private void Awake()
    {
        spriteRenderer= GetComponentInChildren<SpriteRenderer>();
        _rigidbody=GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        //���� ����ü�� �غ���� �ʾҴٸ� �׳� ���ư���
        if (!isReady) return;

        //���� �غ�Ǿ��ٸ� �ð��� �帣�� �ϰ� 
        currentDuration += Time.deltaTime;

        //����ü�� ���ӽð��� rangedWeaponHandler���� ���� ���ӽð��� �Ѿ�� ����
        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false); //���� �޼��� ����
        }

        //�ƴ϶�� �׳� ���ư��� ������ �α�
        _rigidbody.velocity = direction * rangeWeaponHandler.Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���̾ �浹�� ������Ʈ�� ���̾ŭ ��� �о ���� �������� ����ü�� ���̾�� ���� ���
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            //����ü �ı�, �� ��� ���� �ε����Ŷ� �浹�������� ��¦ �ڷ� ���� ��ġ���� ��ƼŬ ����
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * 0.2f, fxOnDestroy);
        }
        //���̾ �浹�� ������Ʈ�� ���̾ŭ ��� �о ���� �������� rangeWeaponHandler�� Ž�� ���̾�� ���� ���
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            //�浹�� ��ü�� ���ҽ���Ʈ�ѷ��� ������ ��������
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                //����ü ���ݷ¸�ŭ ü�� ����
                resourceController.ChangeHealth(-rangeWeaponHandler.Power);
                if (rangeWeaponHandler.IsOnKnockback) //�׸��� �˹鵵 �ִٸ�
                {
                    //��Ʈ�ѷ��� �ҷ��ͼ�
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        //�˹� ����
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }   
            }
            //����ü �ı�, ��. �� ��� ���� ����Ŵ� �� �ڸ��� ��ƼŬ ����
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestroy);
        }
    }

    // ����ü �����ڰ� rangeWeaponHandler�� ������ �� �ְ� �Ǵ� �޼���
    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    {
        //�Է¹��� �Ű������� �����ϰ� �� ���� ���缭 �����ϱ�
        this.projectileManager = projectileManager;
        rangeWeaponHandler = weaponHandler;
        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * weaponHandler.BulletSize;
        spriteRenderer.color = weaponHandler.ProjectileColor;

        //������ ���� �� ��ũ��Ʈ�� �������� �����ϱ�
        transform.right = this.direction;

        if(direction.x<0)
        {
            pivot.localRotation = Quaternion.Euler(180, 0, 0);
        }
        else
        {
            pivot.localRotation = Quaternion.Euler(0, 0, 0);   
        }
        isReady = true; 
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            projectileManager.CreateImpactParticlesAtPosition(position, rangeWeaponHandler);
        }
        Destroy(this.gameObject);
    }
}
