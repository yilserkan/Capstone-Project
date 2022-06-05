using System;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField] private InputField username;
    [SerializeField] private InputField password;
    [SerializeField] private InputField passwordValidation;

    [SerializeField] private GameObject loginPage;
    [SerializeField] private GameObject registerPage;

    private void Start() 
    {
        DatabaseConnection.registerWasSuccesful += HandleRegisterWasSuccesful;
    }
    private void OnDestroy()
    {
        DatabaseConnection.registerWasSuccesful -= HandleRegisterWasSuccesful;
    }

    private void HandleRegisterWasSuccesful()
    {
        registerPage.SetActive(false);
        loginPage.SetActive(true);
    }

    public void RegisterUser()
    {
        if(password.text == passwordValidation.text)
        {
            StartCoroutine(DatabaseConnection.RegisterUser(username.text, password.text));
            return;
        }
        Debug.Log("Passwords are not same");
    }
}
