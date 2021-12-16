using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;

// 점수와 게임 오버 여부를 관리하는 게임 매니저
public class GameManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {   // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {   // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance; // 싱글톤 오브젝트 반환
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    public PlayerController player; // 플레이어

    private int score = 0; // 현재 게임 점수
    //private string userID; // 유저 ID
    public bool isGameover { get; private set; } // 게임 오버 상태
    public bool isGameclear { get; private set; } // 게임 클리어 상태

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 생성할 랜덤 위치 지정
        //Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        // 위치의 y값 0으로 변경
        //randomSpawnPos.y = 0f;
        // 네트워크 상의 모든 클라이언트에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은 생성 메서드를 직접 실행한 클라이언트에게 있음
        //UIManager.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
        // 플레이어 캐릭터의 사망 이벤트 발생 시 게임 오버
        FindObjectOfType<PlayerHealth>().onDeath += EndGame;
    }

    // 점수 추가 & UI 갱신
    public void AddScore(int newScore)
    {
        // 게임 종료가 아닌 상태에서만 점수 증가 가능
        if (!isGameover || !isGameclear)
        {
            score += newScore; // 점수 추가
            // 점수 UI 텍스트 갱신
            UIManager.instance.UpdateScoreText(score);
        }
    }

    public void ClearGame()
    {
        isGameclear = true;
        UIManager.instance.SetActiveGameClearUI(true);
        player.isclear = true;
        //UIManager.instance.insert(score, userID);
    }

    public void EndGame()
    {
        // 게임 오버 상태를 참으로 변경
        isGameover = true;
        // 게임 오버 UI 활성화
        UIManager.instance.SetActiveGameoverUI(true);
    }


    private void Update()
    {
        //userID = Login.userNameData;
        if (isGameover && Input.GetKeyDown(KeyCode.K)) UIManager.instance.GameRestart();
    }
}
