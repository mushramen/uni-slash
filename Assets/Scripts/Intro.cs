using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    // public GameObject gameStartButton;
    public GameObject howToPlayButton;
    public GameObject howToPlayPanel;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonEvent()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnHowToPlayButtonEvent()
    {
        howToPlayPanel.SetActive(true);
    }
}
