using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float rate;
    //public BoxCollider attackArea;
    public BoxCollider attackRange;
    //public TrailRenderer trailEffect;
    public AudioClip attackClip;
    private AudioSource playerAudioPlayer;
    public Weapon weapon;

    private void Update()
    {
        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI()
    {
        UIManager.instance.UpdateDamage(damage);
    }

    public void doSwing()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }


    IEnumerator Swing()
    {
        playerAudioPlayer = GetComponent<AudioSource>();
        yield return new WaitForSeconds(0.1f);
        //attackArea.enabled = true;
        attackRange.enabled = true;
        //trailEffect.enabled = true;
        playerAudioPlayer.PlayOneShot(attackClip);
        yield return new WaitForSeconds(0.5f);
        //yield return new WaitForSeconds(0.3f);
        //trailEffect.enabled = false;
        attackRange.enabled = false;
        yield return new WaitForSeconds(0.1f);
        //attackArea.enabled = false;
    }
}
