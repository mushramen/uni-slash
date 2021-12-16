using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public PlayerController player;
    public GameObject skillsword;
    public Attack playerattack;
    public float damage;
    public Collider isFloor;
    public GameObject skilleffect;

    private void Awake()
    {
        damage = playerattack.damage * 5;
    }

}
