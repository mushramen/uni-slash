using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;
    public Text healthText;
    public Image profile;
    

    public AudioClip deathClip;
    public AudioClip hitClip;

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;

    private PlayerController playerController;
    private GameObject haveWeapon;
    private Weapon weapon;

    private float maxhealth;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        weapon = GetComponent<Weapon>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);
        //healthSlider.maxValue = startingHealth;
        maxhealth = startingHealth;
        healthSlider.value = health / maxhealth;
        healthText.text = health + " / " + startingHealth;
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);

        Debug.Log("newHealth : " + newHealth);
        Debug.Log("health : " + health);
        if (health > maxhealth)
            maxhealth = health;
        healthSlider.value = health / maxhealth;
        healthText.text = (int)health + " / " + (int)maxhealth;
    }

    public override void OnDamage(float damage, Vector3 hitDirection)
    {
        if (!dead && canDamage) playerAudioPlayer.PlayOneShot(hitClip);
        base.OnDamage(damage, hitDirection);
        healthSlider.value = health / maxhealth;
        healthText.text = (int)health + " / " + (int)maxhealth;
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity()의 Die 실행 (사망 적용)
        base.Die();

        // 체력 슬라이더 비활성화
        healthSlider.gameObject.SetActive(false);
        // 사망음 재생
        playerAudioPlayer.PlayOneShot(deathClip);
        // 애니메이터의 Die 트리거를 발동시켜 사망 애니메이션 재생
        playerAnimator.SetTrigger("Die");
        // 플레이어 조작을 받는 컴포넌트 비활성화
        playerController.OnDie();
        healthText.enabled = false;
        GameManager.instance.EndGame();
        profile.color = new Color(80/255f, 80/255f, 80/255f);
    }

}
