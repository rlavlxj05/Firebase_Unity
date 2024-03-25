using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginSystem : MonoBehaviour
{
    public TMP_InputField email, password, name;
    public GameObject loginPn, inGame;
    public TMP_Text outputText;
    GameManager gameManager;
    Player player;
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        player = GameObject.FindObjectOfType<Player>();

        Manager.Instance.Init();
        Manager.Instance.LoginState += OnchangedState;
    }
    private void OnchangedState(bool sign)
    {
        if (sign)
        {
            StartCoroutine(gameManager.Spawn());
            StartCoroutine(player.System());

            loginPn.SetActive(false);
            inGame.SetActive(true);
        }
        else
        {
            loginPn.SetActive(true);
            inGame.SetActive(false);
        }
        outputText.text = sign ? "Login : " : "LogOut : ";
        outputText.text += Manager.Instance.UserId;
    }
    public void Create()
    {
        string e = email.text;
        string p = password.text;
        string n = name.text;
        Manager.Instance.Create(e, p, n);

    }
    public void Login()
    {
        Manager.Instance.Login(email.text, password.text);
    }
    public void Logout()
    {
        Manager.Instance.Logout();
    }
    public void IncreaseScore()
    {
        string s = Manager.Instance.UserId;
        Manager.Instance.Score(s);
    }
}