using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab; // 생성할 적 AI
    public Enemy enemy2Prefab; // 생성할 적 AI
    public Boss bossPrefab;
    public Transform[] spawnPoints; // 적 AI를 소환할 위치들
    public BGMManager bgmmanager; // 보스 등장 시 BGM 변경
    public Attack playerattack; // 적 사망 시 무기 데미지 증가를 위한 공격 스크립트

    public float damageMax = 20; // 최대 공격력
    public float damageMin = 5; // 최소 공격력

    public float healthMax = 100; // 최대 체력
    public float healthMin = 50; // 최소 체력

    public float speedMax = 12f; // 최대 속도
    public float speedMin = 8f; // 최소 속도

    public int maxStage; // 최대 스테이지

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트
    private int stage; // 현재 스테이지

    bool bossAppear;

    private void Start()
    {
        bossAppear = false;
    }

    private void Update()
    {
        // 마지막 스테이지 + 적을 모두 물리치면 보스 스폰
        if(!bossAppear && stage == maxStage && enemies.Count <= 0)
        {
            SpawnBoss();
            bossAppear = true;
        }
        // 적을 모두 물리친 경우 다음 스폰 실행
        if (!bossAppear && enemies.Count <= 0)
        {
            SpawnWave();
        }
        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI()
    {
        UIManager.instance.UpdateStage(stage);
    }

    // 현재 웨이브에 맞춰 적 생성
    private void SpawnWave()
    {
        // 웨이브 1 증가
        stage++;
        // 현재 웨이브 * 4.5를 반올림한 수 만큼 적 생성
        int spawnCount = Mathf.RoundToInt(stage * 4.5f);
        Debug.Log("Spawn Count : " + spawnCount);
        // spawnCount만큼 적 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 적의 세기를 0% ~ 100% 사이에서 랜덤 결정
            float enemyIntensity = Random.Range(0f, 1f);
            Debug.Log("enemy intensity : " + enemyIntensity);
            // 적 생성 처리 실행
            if(enemyIntensity >= 0.7f)
                CreateEnemy(enemyIntensity, 1);
            else CreateEnemy(enemyIntensity, 0);
        }
    }

    // 적을 생성하고 생성한 적에게 추적할 대상 할당
    private void CreateEnemy(float intensity, int enemytype)
    {
        float health;
        float damage;
        float speed;
        Enemy enemy;

        // 생성할 위치 랜덤 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // A타입 적이면 intensity를 기반으로 적의 능력치 결정
        if (enemytype == 0)
        {
            health = Mathf.Lerp(healthMin, healthMax, intensity);
            damage = Mathf.Lerp(damageMin, damageMax, intensity);
            speed = Mathf.Lerp(speedMin, speedMax, intensity);

            enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        }
        else // B타입 적이면
        {
            health = healthMax * intensity;
            damage = damageMax * intensity;
            speed = speedMax * intensity;

            enemy = Instantiate(enemy2Prefab, spawnPoint.position, spawnPoint.rotation);
        }
        
        // 생성한 적의 능력치와 추적 대상 설정
        enemy.Setup(health, damage, speed, enemytype);
        enemy.enabled = false;
        enemy.enabled = true;
        // 생성된 적 리스트에 추가
        enemies.Add(enemy);

        // 적의 onDeath 이벤트에 익명 메서드 등록
        // 사망한 적을 리스트에서 제거
        enemy.onDeath += () => enemies.Remove(enemy);
        // 적 사망 시 점수 상승
        enemy.onDeath += () => GameManager.instance.AddScore(100);
        enemy.onDeath += () => playerattack.damage += 2;
        // 사망한 적 3초 뒤 파괴
        enemy.onDeath += () => Destroy(enemy.gameObject, 3f);
    }

    private void SpawnBoss()
    {
        Vector3 spawnPoint = new Vector3(-34f, 1f, 110f); // 맵 중앙
        Boss boss = Instantiate(bossPrefab, spawnPoint, Quaternion.identity);
        Debug.Log("Boss Setup");
        bgmmanager.ChangeBGM(BGMType.Boss);
    }
}
