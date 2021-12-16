using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    Vector3 moveVec;
    Vector3 rollVec;
    Vector3 skillVec;
    Rigidbody rigid;

    GameObject collisionObject;
    GameObject haveweapon;
    Weapon weapon;
    Attack playerattack;
    public GameObject skillsword;
    public Transform skillposition;
    public GameObject skilleffect;
    public Transform effectposition;
    public Bullet enemybullet;
    public float enemybomb = 30f;
    public CameraShake camerashake;

    PlayerHealth playerHealth;

    public int score = 0;

    public float speed;
    float hAxis;
    float vAxis;

    bool isAttack;
    bool isAttackReady;
    bool isWalk;
    bool isRoll;
    bool isSkill;
    bool isWall;
    bool isdead;
    public bool isclear;

    bool isCoolRoll;
    bool isCoolSkill;

    bool Attackkey;
    bool Rollkey;
    bool Skillkey;

    float attackDelay;
    public float RollCooltime = 5f;
    public float SkillCooltime = 30f;

    public AudioClip attackClip;
    public AudioClip skillClip;
    public AudioClip itemClip;
    public AudioClip rollClip;
    public AudioClip swordClip;
    private AudioSource playerAudioPlayer;

    public Image RollIcon;
    public Text RollText;
    public Image SkillIcon;
    public Text SkillText;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponentInChildren<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>();
        playerattack = GetComponentInChildren<Attack>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
        skillsword.SetActive(false);
    }

    void Start()
    {
        isdead = false;
        isclear = false;
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Roll();
        Attack();
        Skill();
    }
    
    void FixedUpdate()
    {
        FreezeRotation();
        StopWall();
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopWall()
    {
        isWall = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        Attackkey = Input.GetKeyDown(KeyCode.K);
        Rollkey = Input.GetKeyDown(KeyCode.Space);
        Skillkey = Input.GetKeyDown(KeyCode.L);
    }

    void Move()
    {
        if(!isdead && !isclear) moveVec = new Vector3(hAxis, 0f, vAxis).normalized;
        if (isRoll) moveVec = rollVec;
        if (isSkill && !isdead && !isclear) moveVec = Vector3.zero;
        if (!isAttackReady && !isdead && !isclear) moveVec = Vector3.zero;

        if(!isWall) transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    IEnumerator delayRollTime(float cool)
    {
        RollIcon.color = Color.gray;
        RollText.enabled = true;
        RollText.text = ((int)cool).ToString();
        while (cool > 1.0f)
        {
            cool -= Time.deltaTime;
            RollText.text = ((int)cool).ToString();
            yield return new WaitForFixedUpdate();
        }
        isCoolRoll = false;
        RollIcon.color = Color.white;
        RollText.enabled = false;
    }

    void Roll()
    {
        if (Rollkey && isCoolRoll)
        {
            Debug.Log("Cool Time : Roll");
        }

        if(Rollkey && !isRoll && !isCoolRoll && !isdead && !isclear)
        {
            rollVec = moveVec;
            speed *= 2;
            anim.SetTrigger("DoRoll");
            playerAudioPlayer.PlayOneShot(rollClip);
            isRoll = true;
            isCoolRoll = true;
            Invoke("RollEnd", 0.5f);
        }
    }

    void RollEnd()
    {
        speed *= 0.5f;
        isRoll = false;
        StartCoroutine(delayRollTime(RollCooltime));
    }

    IEnumerator delaySkillTime(float cool)
    {
        SkillIcon.color = Color.gray;
        SkillText.enabled = true;
        SkillText.text = ((int)cool).ToString();
        while (cool > 1.0f)
        {
            cool -= Time.deltaTime;
            SkillText.text = ((int)cool).ToString();
            yield return new WaitForFixedUpdate();
        }
        isCoolSkill = false;
        SkillIcon.color = Color.white;
        SkillText.enabled = false;
    }

    void Skill()
    {
        if (Skillkey && isCoolSkill)
        {
            Debug.Log("Cool Time : Skill");
        }

        if (Skillkey && !isCoolSkill && !isdead && !isclear)
        {
            isSkill = true;
            anim.SetTrigger("DoSkill");
            skillVec = moveVec;
            skillsword.SetActive(true);
            GameObject instanceSkill = Instantiate(skillsword, skillposition.position, skillposition.rotation);
            skillsword.SetActive(false);
            Vector3 effecttransform = effectposition.position;
            Rigidbody skillrigid = instanceSkill.GetComponent<Rigidbody>();
            Vector3 skillvel = new Vector3(0, -1, 0) * 10f;
            skillrigid.velocity = skillvel;
            playerAudioPlayer.PlayOneShot(skillClip);
            StartCoroutine("swordsound");
            StartCoroutine(swordeffect(effecttransform));
            isCoolSkill = true;
            Destroy(instanceSkill, 4f);
            Invoke("SkillEnd", 1f);
        }
    }
    
    IEnumerator swordeffect(Vector3 pos)
    {
        yield return new WaitForSeconds(1f);
        camerashake.OnCameraShake();
        GameObject instanceeffect = Instantiate(skilleffect, pos, Quaternion.identity);
    }
    
    IEnumerator swordsound()
    {
        yield return new WaitForSeconds(1f);
        playerAudioPlayer.PlayOneShot(swordClip);
    }

    void SkillEnd()
    {
        isSkill = false;
        StartCoroutine(delaySkillTime(SkillCooltime));
    }

    void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = playerattack.rate < attackDelay;

        if(Attackkey && isAttackReady && !isRoll && !isdead && !isclear)
        {
            playerattack.doSwing();
            weapon.doSwing();
            anim.SetTrigger("DoSwing");
            attackDelay = 0;
        }
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.ItemType.Coin:
                    GameManager.instance.AddScore(item.value);
                    break;
                case Item.ItemType.Health:
                    playerHealth.RestoreHealth(item.value);
                    break;
                
            }
            playerAudioPlayer.PlayOneShot(itemClip);
            Destroy(other.gameObject);
        }
        else if(other.tag == "Weapon")
        {
            Item item = other.GetComponent<Item>();
            Debug.Log("Value : " + item.value);
            playerattack.damage += item.value;
            Debug.Log("weapon damage : " + playerattack.damage);
            playerAudioPlayer.PlayOneShot(itemClip);
            Destroy(other.gameObject);
        }

        if(other.tag == "Bullet")
        {
            Debug.Log("Bullet trigger");
            playerHealth.OnDamage(enemybullet.damage, other.transform.position);
            Destroy(other.gameObject);
        }

        if (other.tag == "Bomb")
        {
            Debug.Log("Bomb trigger");
            Vector3 react = transform.position - other.transform.position;
            rigid.AddForce(react * 8, ForceMode.Impulse);
            playerHealth.OnDamage(enemybomb, other.transform.position);
        }
    }

    public void OnDie()
    {
        isdead = true;
        moveVec = Vector3.zero;
    }
}
