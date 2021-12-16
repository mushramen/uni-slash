using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public enum Type { A, B, C };
    public Type enemyType;

    public int maxHealth;
    public int curHealth;

    public LivingEntity targetEntity;
    public LayerMask whatIsTarget;

    public Rigidbody rigid;
    BoxCollider boxcol;
    Material mat;
    public NavMeshAgent nav;
    public Animator anim;
    public AudioSource enemyaudioplayer;
    public AudioClip enemyhitclip;
    public AudioClip enemyattackclip;
    public AudioClip attackhitclip;
    public BoxCollider enemyWeapon;
    public Slider enemyslider;

    public float damage = 10f;
    public float timeBetAttack = 2f;
    private float lastAttackTime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcol = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        enemyaudioplayer = GetComponent<AudioSource>();
        nav.enabled = false;
        nav.enabled = true;
        // Invoke("ChaseStart", 2f);
    }

    public void Setup(float newHealth, float newDamage, float newSpeed, int enemytype)
    {
        if (enemytype == 0) // 타입 설정 : A / B
            enemyType = Type.A;
        else if (enemytype == 1) enemyType = Type.B;
        else enemyType = Type.C;
        if (enemyType != Type.C)
        {
            startingHealth = newHealth; // 시작 체력 설정
            enemyslider.maxValue = startingHealth; // 시작 체력으로 슬라이더 설정
            health = newHealth; // 체력 설정
            enemyslider.value = health; // 현재 체력으로 슬라이더 설정
            damage = newDamage; // 공격력 설정
            nav.speed = newSpeed; // 내비메시 에이전트 이동속도 설정
            Debug.Log(health + " / " + damage + " / " + nav.speed);
        }
    }

    protected bool hasTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    private IEnumerator UpdatePath() // 주기적으로 추적할 대상의 위치를 찾아 경로 갱신
    {
        while (!dead)
        {   // 살아있는 동안 무한 루프
            if (hasTarget)
            {   // 추적 대상 존재 -> 경로 갱신, AI 이동 계속 진행
                nav.isStopped = false;
                nav.SetDestination(targetEntity.transform.position);
                anim.SetBool("isRun", true);
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
            else
            {   // 추적 대상 없음 : AI 이동 중지
                nav.isStopped = true;
                anim.SetBool("isRun", false);

                // 50 유닛의 반지름을 가진 가상의 구를 그렸을 때 구와 겹치는 모든 콜라이더를 가져옴
                // whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
                Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, whatIsTarget);
                // 모든 콜라이더를 순회하면서 살아있는 LivingEntity 찾기
                for (int i = 0; i < colliders.Length; i++)
                {
                    // 콜라이더로부터 LivingEntity 컴포넌트 가져오기
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    // LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아있다면
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity; // 추적 대상을 해당 LivingEntity로 설정
                        break; // for문 루프 즉시 정지
                    }
                }
            }
            yield return new WaitForSeconds(0.25f); // 0.25초 주기로 처리 반복
        }
    }

    private void Start()
    {
        if(enemyType != Type.C)
            StartCoroutine(UpdatePath());
    }
    /// <summary>
    /// //////
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack")
        {
            //Weapon weapon = other.GetComponent<Weapon>();
            Attack playerattack = other.GetComponent<Attack>();
            Vector3 reactVec = transform.position - other.transform.position;
            OnDamage(playerattack.damage, reactVec);
        }
        if (other.tag == "Skill")
        {
            Skill skill = other.GetComponent<Skill>();
            Vector3 reactVec = transform.position - other.transform.position;
            OnDamage(skill.damage, reactVec);
        }
    }

    public override void OnDamage(float damage, Vector3 hitNormal)
    {
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        if (!dead && canDamage)
        {
            enemyaudioplayer.PlayOneShot(attackhitclip);
            enemyaudioplayer.PlayOneShot(enemyhitclip);
        }
        base.OnDamage(damage, hitNormal);
        if (enemyType != Type.C)
        {
            enemyslider.value = health; // 슬라이더 체력 반영
            rigid.AddForce(hitNormal * 2, ForceMode.Impulse); // 보스가 아니면 넉백
        }
    }

    public override void Die() // 사망 처리
    {   // LivingEntity의 Die()를 실행하여 기본 사망처리 실행
        base.Die();
        // AI 추적 중지 후 내비메시 컴포넌트 비활성화
        nav.isStopped = true;
        nav.enabled = false;
        // 사망 애니메이션 재생
        anim.SetTrigger("Die");
        if (enemyType != Type.C)
            enemyslider.gameObject.SetActive(false); // 비활성화
        gameObject.layer = 14;
        Debug.Log("enemy layer change into 14");
        //Destroy(gameObject, 5f);
    }
    
    private void OnTriggerStay(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행
        // 자신이 사망하지 않았으며 
        // 최근 공격시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
        if (enemyType != Type.C && !dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // 상대방의 LivingEntity 타입 가져오기 시도
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            // 상대방의 Livingentity가 자신의 추적 대상이면 공격 실행
            if (attackTarget != null && attackTarget == targetEntity)
            {
                // 최근 공격 시간 갱신
                lastAttackTime = Time.time;
                // 상대방 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitNormal = transform.position - other.transform.position;
                // 공격 실행
                enemyWeapon.enabled = true;
                anim.SetTrigger("DoAttack");
                attackTarget.OnDamage(damage, hitNormal);
                if (enemyType != Type.C)
                    enemyaudioplayer.PlayOneShot(enemyattackclip);
                enemyWeapon.enabled = false;
            }
        }
    }
    
    IEnumerator EnemyAttack(LivingEntity attackTarget, Vector3 hitNormal)
    {
        yield return new WaitForSeconds(1f);
        enemyWeapon.enabled = true;
        anim.SetTrigger("DoAttack");
        attackTarget.OnDamage(damage, hitNormal);
        enemyWeapon.enabled = false;
    }
}
