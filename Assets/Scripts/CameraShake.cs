using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] public float shakeintensity = 0f;
    [SerializeField] public float shaketime = 0f;

    // 싱글톤 접근용 프로퍼티
    public static CameraShake instance
    {
        get
        {   // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {   // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<CameraShake>();
            }
            return m_instance; // 싱글톤 오브젝트 반환
        }
    }

    private static CameraShake m_instance; // 싱글톤이 할당될 static 변수

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    public void OnCameraShake(float shaketime = 0.75f, float shakeintensity = 0.5f)
    {
        
        this.shaketime = shaketime;
        this.shakeintensity = shakeintensity;
        StopCoroutine("ShakeCoroutine2");
        StartCoroutine("ShakeCoroutine2");
    }

    IEnumerator ShakeCoroutine()
    {
        Vector3 startPosition = transform.position;
       // float shaketimenow = shaketime;
        while (shaketime > 0.0f)
        {
            float rotX = Random.Range(-1f, 1f);
            // float rotY = Random.Range(-1f, 1f);
            float rotZ = Random.Range(-1f, 1f);

            transform.position = startPosition + Random.insideUnitSphere * shakeintensity;
            // 초기 위치로부터 구 범위 * 흔드는 세기의 범위 안에서만 카메라 흔들기

            shaketime -= Time.deltaTime;

            yield return null;
        }
        transform.position = startPosition; // 초기 위치로 복귀
    }

    IEnumerator ShakeCoroutine2()
    {
        Vector3 startRotation = transform.eulerAngles;
        float force = 5f;
        while (shaketime > 0.0f)
        {
            float x = Random.Range(-1f, 1f);
            float y = 0; // Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);

            transform.rotation = Quaternion.Euler(startRotation + new Vector3(x, y, z) * shakeintensity * force);
            
            shaketime -= Time.deltaTime;

            yield return null;
        }
        transform.rotation = Quaternion.Euler(startRotation); // 초기 위치로 복귀
    }
}
