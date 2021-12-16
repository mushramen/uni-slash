using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // 내비메쉬 관련 코드

// 주기적으로 아이템을 플레이어 근처에 생성하는 스크립트
public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items; // 생성할 아이템들
    public Transform playerTransform; // 플레이어의 트랜스폼

    public float maxDistance = 5f; // 플레이어 위치로부터 아이템이 배치될 최대 반경

    public float timeBetSpawnMax = 15f; // 최대 시간 간격
    public float timeBetSpawnMin = 5f; // 최소 시간 간격
    private float timeBetSpawn; // 생성 간격

    private float lastSpawnTime; // 마지막 생성 시점

    void Start()
    {
        // 생성 간격과 마지막 생성 시점 초기화
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
    }

    // 주기적으로 아이템 생성
    void Update()
    {
        if (Time.time >= lastSpawnTime + timeBetSpawn)
        {
            lastSpawnTime = Time.time; // 마지막 생성 시간 갱싱
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax); // 생성 주기를 랜덤으로 변경
            // 아이템 생성 실행
            Spawn();
        }
    }

    // 아이템 생성
    private void Spawn()
    {
        Vector3 spawnPosition = GetRandomPointOnNavMesh(new Vector3(-40f, 0f, 94f), maxDistance);
        // 바닥에서 1만큼 위로 올리기
        spawnPosition += Vector3.up * 1f;
        // 아이템 중 하나를 무작위로 골라 랜덤 위치에 생성
        GameObject ItemToCreate = items[Random.Range(0, items.Length)];
        // 인스턴스 생성
        GameObject item = Instantiate(ItemToCreate, spawnPosition, Quaternion.identity);

        // 생성된 아이템 15초 뒤 파괴
        StartCoroutine(DestroyAfter(item, 15f));
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        // 딜레이만큼 대기
        yield return new WaitForSeconds(delay);
        // 아이템이 파괴되지 않았으면 파괴
        if (target != null)
        {
            Destroy(target);
        }
    }

    // 내비메시 위의 랜덤한 위치를 반환하는 메서드
    // center를 중심으로 distance 반경 안에서 랜덤한 위치를 찾음
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        // center를 중심으로 반지름이 maxDistance인 구 안에서의 랜덤한 위치 하나 저장
        // Random.insideUnitSphere는 반지름이 1인 구 안에서의 랜덤한 한 점을 반환하는 프로퍼티
        Vector3 randomPos = Random.insideUnitSphere * distance + center;
        Debug.Log("randomPos : " + randomPos.x + " " + randomPos.z);
        NavMeshHit hit; // 내비메시 샘플링의 결과 정보 저장 변수
        // maxDistance 반경 안에서 randomPos에 가장 가까운 내비메시 위의 한 점을 찾음
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);

        // 찾은 점 반환
        return hit.position;
    }
}
