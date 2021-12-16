using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataInserter : MonoBehaviour
{
    public InputField inputUserName;
    public InputField inputPW;
    public GameObject createUserInfo;
    public GameObject createAccountPanel;

    string CreateUserURL = "127.0.0.1/indexing_unislash.php";

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnCreateButtonClickEvent()
    {
        CreateUser(inputUserName.text, inputPW.text);
    }

    public void CreateUser(string username, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", pw);

        WWW www = new WWW(CreateUserURL, form);

        createAccountPanel.SetActive(false);
    }
}
