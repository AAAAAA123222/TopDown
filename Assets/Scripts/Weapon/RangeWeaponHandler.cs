using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1f;
    public float BulletSize { get {  return bulletSize; } }

    [SerializeField] private float duration;
    public float Duration { get { return duration; } }

    [SerializeField] private float spread;
    public float Spread { get { return spread; } }

    [SerializeField] private int numberofProjectilesPerShot;
    public int NumberofProjectilesPerShot { get {  return numberofProjectilesPerShot; } }

    [SerializeField] private float multipleProjectileAngle;
    public float MultipleProjectileAngle { get {  return multipleProjectileAngle; } }

    [SerializeField] private Color projectileColor;
    public Color ProjectileColor { get { return projectileColor; } }

    private ProjectileManager projectileManager;
    protected override void Start()
    {
        base.Start();
        projectileManager = ProjectileManager.Instance;
    }
    public override void Attack()
    {
        base.Attack();
        float projectileAngleSpace = multipleProjectileAngle;
        int numberOfProjectilePerShot= numberofProjectilesPerShot;

        float minAngle = -(numberOfProjectilePerShot / 2f) * projectileAngleSpace;

        for(int i=0;i<numberOfProjectilePerShot;i++)
        {
            float angle = minAngle + projectileAngleSpace * i;
            float randomSpread = Random.Range(-spread, spread);
            angle += randomSpread;
            CreateProjectile(Controller.LookDirection, angle);
        }

    }

    private void CreateProjectile(Vector2 _lookDirection, float angle)
    {
        //투사체 관리자에서 사격 메서드 호출
        projectileManager.ShootBullet(
            this, //이 스크립트에서
            projectileSpawnPosition.position, //투사체가 발사되는 지점을 기점으로
            RotateVector2(_lookDirection, angle) //lookdirection에서 angle만큼 회전시킨 값으로 발사하겠다
            );
    }

    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        //쿼터니언의 각도만큼 벡터 회전시키기
        return Quaternion.Euler(0, 0, degree) * v;
    }


}
