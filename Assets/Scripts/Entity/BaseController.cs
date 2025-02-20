using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    //??�̰ſּ��������???????????????????????
    //�ν����Ϳ��� �����Ҽ��ֵ��� private �տ� Serializefield �ۼ�
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    //���� ���� �̵��� ���� ���� �ʵ�� ���� ����
    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }
    //�˹�� ���ӽð� �ʵ� ����
    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;

    [SerializeField] public WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler=GetComponent<AnimationHandler>();
        statHandler=GetComponent<StatHandler>();

        //���� �������� ������� �ʴٸ� weaponPivot�� weaponPrefab�� ������ �� ������ ����
        if (WeaponPrefab != null)
        {
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        }
        else //����ִٸ� �ڽ� ��ü���� ã��
        {
            weaponHandler=GetComponentInChildren<WeaponHandler>();
        }
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        //�Է°� �̵�, �ٶ󺸴� �������� ȸ���ϴ� �Լ��� ����ؼ� ����
        HandleAction();
        Rotate(lookDirection);
        HandleAttackDelay();
    }
    protected virtual void FixedUpdate()
    {
        //�̵� ����
        Movement(movementDirection);
        //���� �˹����̸� �˹� �ð��� ���̱�
        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleAction()
    {

    }
    private void Movement(Vector2 direction)
    {
        //�̵����⿡ 5��
        direction = direction * statHandler.Speed;
        if(knockbackDuration>0.0f)//���� �˹����̸�
        {
            direction *= 0.2f; //�ٽ� ���̰�
            direction += knockback; //�˹� �ֱ�
        }

        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    private void Rotate(Vector2 direction)
    {
        //��ũ ź��Ʈ 2 * Rad2Deg
        //y���� x���� �ް� �� ���� ���� �޾ƿ��� �� ���� ���� ���ϰ� ������ �����ϴ� ����
        //�� �̰� �۵��ϴ��� ���� ���� �Ƹ� ��� ���ϰ���
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //���� ���� �޾Ҵµ� 90������ ũ�� ������ �ٶ󺸴� ���� �ۼ�
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        //�� ���� ���� X�� ������
        characterRenderer.flipX = isLeft;

        //���� ���Ⱑ �ִٸ�
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ); //���⵵ ������
        }
        weaponHandler?.Rotate(isLeft);
    }
    
    //�˹��� �󸶳� �� �� ���ϴ� �޼���
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration; //�˹� �ð� 
        //�˹���� ����� �Ÿ��� ���� �� �Ÿ��� �ʱ�ȭ�ϰ� �� ��� �˹� ��ݷ��� ���ϱ�
        //others ��ġ�� transform�� ��ġ�� ���� ����
        //others ��ġ�� transform�� ��ġ�� ���� transform���� others�� ���ϴ� ����� �Ÿ��� ���� ���� ������ ����
        knockback = -(other.position - transform.position).normalized * power; 
    }

    private void HandleAttackDelay()
    {
        //�������� ���ٸ�
        if(weaponHandler==null)
        {
            //�׳� ���ư���
            return;
        }

        //���ð��� �����̺��� ���ٸ�
        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            //��ٸ� �ð���ŭ ������ �� �߰�
            timeSinceLastAttack += Time.deltaTime;
        }

        //���� �����ϰ� �ְ� ���� �����̺��� ���� ��ٷȴٸ�
        if (isAttacking&&timeSinceLastAttack>weaponHandler.Delay)
        {
            //���ð� �ʱ�ȭ �� ����
            timeSinceLastAttack = 0;
            Attack();
        }

    }

    protected virtual void Attack()
    {
        if (lookDirection!=Vector2.zero)
        {
            weaponHandler?.Attack();
        }
    }

    public virtual void Death()
    {
        _rigidbody.velocity = Vector3.zero;

        //��� ��������Ʈ�� ����ͼ�
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            //�������ϰ� �����
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        //�׸���, ��� ������Ʈ�� ��Ȱ��ȭ ��Ų ��
        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }
        
        //2�� �� ����
        Destroy(gameObject, 2f);
    }


}
