using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public InputField inputUserName;
    public InputField inputPW;
    public GameObject loginButton;
    public GameObject createAccountButton;
    public GameObject createAccountPanel;

    public static string userNameData;
    public static string userPWData;

    string LoginURL = "127.0.0.1/login_unislash.php";

    void Start()
    {
        //Screen.SetResolution(1280, 720, true); // 1280 * 720 고정
    }

    void Update()
    {
        userNameData = inputUserName.text;
        userPWData = inputPW.text;
    }

    public void OnLoginButtonClickEvent()
    {
        StartCoroutine(LoginToDB(inputUserName.text, inputPW.text));
        //StartCoroutine(LoginToDB(inputUserName.text));
        Debug.Log(inputUserName + " Log In");
        Debug.Log(inputPW);
    }

    public void OnButtonEvent()
    {
        SceneManager.LoadScene("Main");
    }
    
    public void OnCreateAccountButtonEvent()
    {
        createAccountPanel.SetActive(true);
    }
    
    IEnumerator LoginToDB(string username, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("namePost", username);
        form.AddField("passPost", pw);

        Debug.Log(username + " Log In to DB");
        Debug.Log(pw);

        WWW www = new WWW(LoginURL, form);

        yield return www;
        Debug.Log(www.text);

        
        if(www.text == "login success")
        {
            SceneManager.LoadScene("Main");
        }
        
    }
}
