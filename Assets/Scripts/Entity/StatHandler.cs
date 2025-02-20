using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    // 필드랑 get/set 지정하고 최댓값까지 정하기
    [Range(1, 1000)][SerializeField] private int health = 10;
    public int Health
    {
        get => health;
        set=>health=Mathf.Clamp(value, 0, 100); 
    }

    [Range(1, 20f)][SerializeField] private float speed = 3;
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, 20);

    }

}
