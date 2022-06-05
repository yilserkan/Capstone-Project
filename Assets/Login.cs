using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private InputField username;
    [SerializeField] private InputField password;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject landingPanel;

    private string loggedInSuccesfully = "";

    private void Start() 
    {
        DatabaseConnection.loginWasSuccesful += HandleLoginWasSucessful;
    }

    private void OnDestroy()
    {
        DatabaseConnection.loginWasSuccesful -= HandleLoginWasSucessful;
    }

    public void TryLogin()
    {
        if(username.text.Equals("") || password.text.Equals("")) { return; }

        StartCoroutine(DatabaseConnection.Login(username.text, password.text));
    }

    private void HandleLoginWasSucessful()
    {
        loginPanel.SetActive(false);
        landingPanel.SetActive(true);
    }
}
