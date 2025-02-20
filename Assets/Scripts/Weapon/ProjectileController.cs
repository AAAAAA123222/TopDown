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

    //삭제될 때 이펙트 출력하기
    public bool fxOnDestroy = true;

    private void Awake()
    {
        spriteRenderer= GetComponentInChildren<SpriteRenderer>();
        _rigidbody=GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        //만약 투사체가 준비되지 않았다면 그냥 돌아가기
        if (!isReady) return;

        //만약 준비되었다면 시간을 흐르게 하고 
        currentDuration += Time.deltaTime;

        //투사체의 지속시간이 rangedWeaponHandler에서 정한 지속시간을 넘어가는 순간
        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false); //삭제 메서드 실행
        }

        //아니라면 그냥 날아가게 내버려 두기
        _rigidbody.velocity = direction * rangeWeaponHandler.Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //레이어를 충돌한 오브젝트의 레이어만큼 계속 밀어서 나온 이진값이 투사체의 레이어와 같을 경우
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            //투사체 파괴, 이 경우 벽에 부딪힌거라 충돌지점에서 살짝 뒤로 빠진 위치에서 파티클 생성
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * 0.2f, fxOnDestroy);
        }
        //레이어를 충돌한 오브젝트의 레이어만큼 계속 밀어서 나온 이진값이 rangeWeaponHandler의 탐색 레이어와 같을 경우
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            //충돌한 객체의 리소스컨트롤러를 변수로 가져오기
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                //투사체 공격력만큼 체력 변경
                resourceController.ChangeHealth(-rangeWeaponHandler.Power);
                if (rangeWeaponHandler.IsOnKnockback) //그리고 넉백도 있다면
                {
                    //컨트롤러도 불러와서
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        //넉백 적용
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }   
            }
            //투사체 파괴, 단. 이 경우 적을 맞춘거니 그 자리에 파티클 생성
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestroy);
        }
    }

    // 투사체 제어자가 rangeWeaponHandler를 참조할 수 있게 되는 메서드
    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    {
        //입력받은 매개변수를 저장하고 그 값에 맞춰서 조정하기
        this.projectileManager = projectileManager;
        rangeWeaponHandler = weaponHandler;
        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * weaponHandler.BulletSize;
        spriteRenderer.color = weaponHandler.ProjectileColor;

        //오른쪽 축을 이 스크립트의 방향으로 설정하기
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
