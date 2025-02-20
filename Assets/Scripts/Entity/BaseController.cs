using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    //??이거왜설명생략함???????????????????????
    //인스펙터에서 설정할수있도록 private 앞에 Serializefield 작성
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    //보는 곳과 이동할 곳에 대한 필드와 게터 선언
    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }
    //넉백과 지속시간 필드 선언
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

        //만약 프리팹이 비어있지 않다면 weaponPivot에 weaponPrefab을 복제한 뒤 변수에 저장
        if (WeaponPrefab != null)
        {
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        }
        else //비어있다면 자식 객체에서 찾기
        {
            weaponHandler=GetComponentInChildren<WeaponHandler>();
        }
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        //입력과 이동, 바라보는 방향으로 회전하는 함수를 계속해서 실행
        HandleAction();
        Rotate(lookDirection);
        HandleAttackDelay();
    }
    protected virtual void FixedUpdate()
    {
        //이동 실행
        Movement(movementDirection);
        //만약 넉백중이면 넉백 시간을 줄이기
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
        //이동방향에 5배
        direction = direction * statHandler.Speed;
        if(knockbackDuration>0.0f)//만약 넉백중이면
        {
            direction *= 0.2f; //다시 줄이고
            direction += knockback; //넉백 넣기
        }

        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    private void Rotate(Vector2 direction)
    {
        //아크 탄젠트 2 * Rad2Deg
        //y값과 x값을 받고 그 사이 값을 받아오고 그 값을 쓰기 편하게 각도로 정제하는 수식
        //왜 이게 작동하는지 이해 못함 아마 평생 못하겠지
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //이후 절댓값 받았는데 90도보다 크면 왼쪽을 바라보는 변수 작성
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        //그 값에 따라 X값 뒤집기
        characterRenderer.flipX = isLeft;

        //만약 무기가 있다면
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ); //무기도 돌리기
        }
        weaponHandler?.Rotate(isLeft);
    }
    
    //넉백을 얼마나 줄 지 정하는 메서드
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration; //넉백 시간 
        //넉백당할 방향과 거리를 구한 뒤 거리를 초기화하고 그 대신 넉백 충격량을 곱하기
        //others 위치에 transform의 위치를 뺴는 이유
        //others 위치에 transform의 위치를 빼면 transform에서 others로 향하는 방향과 거리에 대한 값이 나오기 때문
        knockback = -(other.position - transform.position).normalized * power; 
    }

    private void HandleAttackDelay()
    {
        //무기조차 없다면
        if(weaponHandler==null)
        {
            //그냥 돌아가기
            return;
        }

        //대기시간이 딜레이보다 적다면
        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            //기다린 시간만큼 변수에 값 추가
            timeSinceLastAttack += Time.deltaTime;
        }

        //만약 공격하고 있고 무기 딜레이보다 많이 기다렸다면
        if (isAttacking&&timeSinceLastAttack>weaponHandler.Delay)
        {
            //대기시간 초기화 및 공격
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

        //모든 스프라이트를 갖고와서
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            //반투명하게 만들기
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        //그리고, 모든 컴포넌트를 비활성화 시킨 뒤
        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }
        
        //2초 뒤 삭제
        Destroy(gameObject, 2f);
    }


}
