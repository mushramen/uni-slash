using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : Enemy
{
    public GameObject patternsword;
    public Transform pattern1spot1;
    public Transform pattern1spot2;
    public Transform pattern1spot3;
    public GameObject pattern1plane1;
    public GameObject pattern1plane2;
    public GameObject pattern1plane3;
    public Transform swordDestination;
    public Slider bossSlider;
    public EnemySpawner enemySpawner;
    public CameraShake camerashake;

    public GameObject bombspot1;
    public GameObject bombspot2;
    public GameObject bombspot3;
    public GameObject bombspot4;
    public GameObject bombspot5;
    public GameObject bombplane;
    public GameObject explosioneffect1;

    public GameObject tauntarea;
    public GameObject taunteffect;
    public GameObject tauntexplosion;

    public AudioClip bossappearclip;
    public AudioClip bosspattern1clip;
    public AudioClip bosspattern2clip;
    public AudioClip bosspattern3clip;
    public AudioClip bossdeadclip;
    public AudioClip swordclip;
    public AudioClip explosionclip;
    public AudioSource bossaudioplayer;

    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook;
    bool isChasing;

    private void Awake()
    {
        isLook = true;
        isChasing = true;
        bossaudioplayer.PlayOneShot(bossappearclip);
        
        StartCoroutine("Think", 1.75f);
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        bossaudioplayer = GetComponent<AudioSource>();
        //camerashake = GetComponent<CameraShake>();
        isLook = true;
        isChasing = true;
        health = startingHealth; // 체력 설정
        BossHealth.instance.SetObject();
        BossHealth.instance.SetSlider(health, startingHealth);
        Debug.Log("Boss Health : " + health);
    }

    private void Update()
    {
        if (!dead && isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 3f;
            lookPlayer();
        }
    }

    IEnumerator UpdatePathBoss()
    {
        float chasetime = 2f;
        float curtime = 0f;
        while (!dead && chasetime > curtime)
        {   // 살아있는 동안 무한 루프
            if (hasTarget)
            {   // 추적 대상 존재 -> 경로 갱신, AI 이동 계속 진행
                nav.isStopped = false;
                nav.SetDestination(targetEntity.transform.position);
                anim.SetBool("isRun", true);
                transform.LookAt(targetEntity.transform.position + lookVec);
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
            else
            {   // 추적 대상 없음 : AI 이동 중지
                nav.isStopped = true;
                anim.SetBool("isRun", false);

                // 20 유닛의 반지름을 가진 가상의 구를 그렸을 때 구와 겹치는 모든 콜라이더를 가져옴
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
            curtime += 0.25f;
            Debug.Log("chasetime : " + curtime);
        }
        anim.SetBool("isRun", false);
        isChasing = false;
        if(!dead)
            StartCoroutine(Think());
    }
    
    void lookPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 200f, whatIsTarget);
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
        transform.LookAt(targetEntity.transform.position + lookVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(1.25f);
        int action = Random.Range(0, 10);
        Debug.Log("range : " + action);
        if (!dead)
        {
            switch (action)
            {
                case 0:
                case 1:
                    isChasing = true;
                    StartCoroutine(UpdatePathBoss());
                    
                    break;
                case 2:
                case 3:
                case 4:
                    StartCoroutine(throwtripleSword());
                    break;
                case 5:
                case 6:
                case 7:
                    StartCoroutine(attackShield());
                    break;
                case 8:
                case 9:
                    StartCoroutine(Taunt());
                    break;
            }
        }
    }


    IEnumerator throwtripleSword()
    {
        anim.SetTrigger("DoAttack");
        pattern1plane1.SetActive(true);
        pattern1plane2.SetActive(true);
        pattern1plane3.SetActive(true);
        bossaudioplayer.PlayOneShot(bosspattern1clip);
        isLook = false;
        yield return new WaitForSeconds(1f);
        GameObject swordinstance1 = Instantiate(patternsword, pattern1spot1.position, pattern1spot1.rotation);
        Rigidbody swordrigid1 = swordinstance1.GetComponent<Rigidbody>();
        GameObject swordinstance2 = Instantiate(patternsword, pattern1spot2.position, pattern1spot2.rotation);
        Rigidbody swordrigid2 = swordinstance2.GetComponent<Rigidbody>();
        GameObject swordinstance3 = Instantiate(patternsword, pattern1spot3.position, pattern1spot3.rotation);
        Rigidbody swordrigid3 = swordinstance3.GetComponent<Rigidbody>();
        bossaudioplayer.PlayOneShot(swordclip);
        pattern1plane1.SetActive(false);
        pattern1plane2.SetActive(false);
        pattern1plane3.SetActive(false);
        swordrigid1.velocity = pattern1spot1.up * 25;
        swordrigid2.velocity = pattern1spot2.up * 25;
        swordrigid3.velocity = pattern1spot3.up * 25;
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator attackShield()
    {
        isLook = false;
        anim.SetTrigger("DoShield");
        bombplane.SetActive(true);
        bossaudioplayer.PlayOneShot(bosspattern2clip);
        yield return new WaitForSeconds(1f);
        bombplane.SetActive(false);
        bombspot1.SetActive(true);
        GameObject instanceexplosion1 = Instantiate(explosioneffect1, bombspot1.transform.position, Quaternion.identity);
        bossaudioplayer.PlayOneShot(explosionclip);
        CameraShake.instance.OnCameraShake(0.25f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        bombspot1.SetActive(false);
        bombspot2.SetActive(true);
        GameObject instanceexplosion2 = Instantiate(explosioneffect1, bombspot2.transform.position, Quaternion.identity);
        bossaudioplayer.PlayOneShot(explosionclip);
        CameraShake.instance.OnCameraShake(0.25f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        bombspot2.SetActive(false);
        bombspot3.SetActive(true);
        GameObject instanceexplosion3 = Instantiate(explosioneffect1, bombspot3.transform.position, Quaternion.identity);
        bossaudioplayer.PlayOneShot(explosionclip);
        CameraShake.instance.OnCameraShake(0.25f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        bombspot3.SetActive(false);
        bombspot4.SetActive(true);
        GameObject instanceexplosion4 = Instantiate(explosioneffect1, bombspot4.transform.position, Quaternion.identity);
        bossaudioplayer.PlayOneShot(explosionclip);
        CameraShake.instance.OnCameraShake(0.25f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        bombspot4.SetActive(false);
        bombspot5.SetActive(true);
        GameObject instanceexplosion5 = Instantiate(explosioneffect1, bombspot5.transform.position, Quaternion.identity);
        bossaudioplayer.PlayOneShot(explosionclip);
        CameraShake.instance.OnCameraShake(0.25f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        bombspot5.SetActive(false);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        isLook = false;
        anim.SetTrigger("DoTaunt");
        bossaudioplayer.PlayOneShot(bosspattern3clip);
        GameObject instanceeffect = Instantiate(taunteffect, tauntarea.transform.position, Quaternion.EulerAngles(-90, 0, 0));
        yield return new WaitForSeconds(1.5f);
        tauntarea.GetComponent<Collider>().enabled = true;
        Destroy(instanceeffect);
        GameObject instanceexplosion = Instantiate(tauntexplosion, tauntarea.transform.position, Quaternion.identity);
        bossaudioplayer.PlayOneShot(explosionclip);
        CameraShake.instance.OnCameraShake(1f, 1f);
        yield return new WaitForSeconds(1f);
        tauntarea.GetComponent<Collider>().enabled = false;
        isLook = true;
        StartCoroutine(Think());
    }

    public override void OnDamage(float damage, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitNormal);
        BossHealth.instance.SetSlider(health, startingHealth);
        // bossSlider.value = health / startingHealth;
    }

    public override void Die() // 사망 처리
    {   // LivingEntity의 Die()를 실행하여 기본 사망처리 실행
        base.Die();
        bossaudioplayer.PlayOneShot(bossdeadclip);
        Debug.Log("Boss Dead");
        BossHealth.instance.OffObject();
        GameManager.instance.ClearGame();
    }
}

