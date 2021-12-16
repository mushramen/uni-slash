using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    private void Awake()
    {
        damage = 50f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Floor" || other.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
