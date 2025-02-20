using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    //무적 시간 정하기
    [SerializeField] private float healthChangeDelay = 0.5f;

    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    private float timeSinceLastChange = float.MaxValue;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => statHandler.Health;

    public AudioClip damageClip;

    private Action<float, float> OnChangeHealth;
    public void Awake()
    {
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    private void Start()
    {
        CurrentHealth=statHandler.Health;
    }

    private void Update()
    {
        //맞은 뒤 시간이 무적 시간보다 작다면
        if (timeSinceLastChange <healthChangeDelay)
        {
            //시간 올리다가
            timeSinceLastChange += Time.deltaTime;
            //드디어 값이 똑같아지면 피격 애니메이션 멈추기
            if (timeSinceLastChange>=healthChangeDelay)
            {
                animationHandler.InvincibilityEnd();
            }
        }
    }

    public bool ChangeHealth(float change)
    {
        //변화값이 0이거나 무적시간이 지나기 전에 또 호출된 상태라면 그냥 취소하기
        if(change==0||timeSinceLastChange<healthChangeDelay)
        {
            return false;
        }

        //마지막 피격까지의 시간 초기화
        timeSinceLastChange = 0f;

        //그리고 체력 바꾸기
        CurrentHealth += change;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
        if (change<0) //값이 음수면(체력이 깎일 상황이라면)
        {
            animationHandler.Damage(); //데미지 애니메이션 호출
            if (damageClip != null)
                SoundManager.PlayClip(damageClip);
        }
        if(CurrentHealth<=0f)  //체력이 0이면
        {
            Death(); //죽기
        }
        return true;
    }

    private void Death()
    {
        baseController.Death();
    }
    public void AddHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth += action;
    }

    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth -= action;
    }
}
