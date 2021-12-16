using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float damage;
    private void Awake()
    {
        damage = 30f;
    }
}
