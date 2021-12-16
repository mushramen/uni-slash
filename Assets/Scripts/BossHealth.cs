using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static BossHealth instance
    {
        get
        {   // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {   // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<BossHealth>();
            }
            return m_instance; // 싱글톤 오브젝트 반환
        }
    }

    private static BossHealth m_instance; // 싱글톤이 할당될 static 변수

    public Slider bossSlider;
    public GameObject bossObject;

    public void SetSlider(float health, float maxhealth)
    {
        bossSlider.value = health / maxhealth;
    }

    public void SetObject()
    {
        bossObject.SetActive(true);
    }

    public void OffObject()
    {
        bossObject.SetActive(false);
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
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
