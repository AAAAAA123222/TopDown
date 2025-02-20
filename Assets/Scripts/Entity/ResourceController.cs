using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    //���� �ð� ���ϱ�
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
        //���� �� �ð��� ���� �ð����� �۴ٸ�
        if (timeSinceLastChange <healthChangeDelay)
        {
            //�ð� �ø��ٰ�
            timeSinceLastChange += Time.deltaTime;
            //���� ���� �Ȱ������� �ǰ� �ִϸ��̼� ���߱�
            if (timeSinceLastChange>=healthChangeDelay)
            {
                animationHandler.InvincibilityEnd();
            }
        }
    }

    public bool ChangeHealth(float change)
    {
        //��ȭ���� 0�̰ų� �����ð��� ������ ���� �� ȣ��� ���¶�� �׳� ����ϱ�
        if(change==0||timeSinceLastChange<healthChangeDelay)
        {
            return false;
        }

        //������ �ǰݱ����� �ð� �ʱ�ȭ
        timeSinceLastChange = 0f;

        //�׸��� ü�� �ٲٱ�
        CurrentHealth += change;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
        if (change<0) //���� ������(ü���� ���� ��Ȳ�̶��)
        {
            animationHandler.Damage(); //������ �ִϸ��̼� ȣ��
            if (damageClip != null)
                SoundManager.PlayClip(damageClip);
        }
        if(CurrentHealth<=0f)  //ü���� 0�̸�
        {
            Death(); //�ױ�
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
