using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 코드
using UnityEngine.SceneManagement; // 씬 관리자 관련 코드

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour
{   // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public Text scoreText; // 점수 표시용 텍스트
    public Text stageText; // 스테이지 표시용 텍스트
    public Text damageText; // 공격력 표시용 텍스트
    public Text scoreresult; // 결과 점수
    public GameObject gameoverUI; // 게임 오버 시 활성화할 UI
    public GameObject gameclearUI; // 게임 클리어 시 활성화할 UI
    public Button restartButton; // 재시작 버튼
    public string sceneName; // 씬 이름
    public Text resultUIscore;
    //public Text resultUIID;

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = newScore.ToString();
    }

    // 스테이지 텍스트 갱신
    public void UpdateStage(int stage)
    {
        stageText.text = stage.ToString();
    }

    // 공격력 텍스트 갱신
    public void UpdateDamage(int newDamage)
    {
        damageText.text = "ATK : " + (int)newDamage;
    }

    // 게임 오버 UI 활성화
    public void SetActiveGameoverUI(bool active)
    {
        scoreresult.text = scoreText.text;
        gameoverUI.SetActive(active);
    }

    // 게임 클리어 UI 활성화
    public void SetActiveGameClearUI(bool active)
    {
        resultUIscore.text = scoreText.text;
        gameclearUI.SetActive(active);
    }

    // 게임 재시작
    public void GameRestart()
    {
        SceneManager.LoadScene(sceneName);
    }
}
