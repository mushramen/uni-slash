using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// 생명체로서 동작할 게임 오브젝트들을 위한 뼈대를 제공
// 체력, 데미지 받아들이기, 사망 기능, 사망 이벤트 제공
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set;  } // 사망 상태
    public event Action onDeath; // 사망시 발동 이벤트
    protected bool canDamage;

    // 생명체 활성화될 때 상태 리셋
    protected virtual void OnEnable()
    {
        // 사망하지 않은 상태로 시작
        dead = false;
        // 체력을 시작 체력으로 초기화
        health = startingHealth;
        // 데미지 입을 수 있음
        canDamage = true;
    }

    // 데미지 입는 기능
    public virtual void OnDamage(float damage, Vector3 hitNormal)
    {
        // 무적 상태가 아니라면 데미지만큼 체력 감소
        if (canDamage)
        {
            health -= damage;
            Debug.Log("Health : " + health);
        }
        // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        if (health <= 0 && !dead) Die();
        if(canDamage) StartCoroutine(NoDamage());
    }

    IEnumerator NoDamage()
    {
        canDamage = false; // 무적
        yield return new WaitForSeconds(0.75f);
        canDamage = true; // 해제
    }

    // 체력 회복 기능
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return; // 이미 사망한 경우 체력회복 불가능
        health += newHealth; // 체력 회복
    }

    // 사망 처리
    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메서드 있으면 실행
        if (onDeath != null) onDeath();
        // 사망 상태 참으로 변경
        dead = true;
    }
}
