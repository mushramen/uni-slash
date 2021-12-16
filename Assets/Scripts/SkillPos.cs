using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPos : MonoBehaviour
{
    public GameObject skilleffect;
    public Collider col;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            col.enabled = true;
            Vector3 pos = other.GetComponent<Transform>().position;
            Debug.Log("other pos : " + pos);
            swordeffect(pos);
        }
    }

    IEnumerator swordeffect(Vector3 pos)
    {
        yield return new WaitForSeconds(1f);
        GameObject instanceeffect = Instantiate(skilleffect, pos, Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
