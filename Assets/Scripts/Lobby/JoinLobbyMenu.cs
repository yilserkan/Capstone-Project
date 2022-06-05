using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager;
    [SerializeField] private TMP_InputField InputField;
    [SerializeField] private Button connectButton;

    [SerializeField] private GameObject landingPage;

    private void OnEnable() 
    {
        MyNetworkManager.OnClientConnected += HandleClientConnected;
        MyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() 
    {
        MyNetworkManager.OnClientConnected -= HandleClientConnected;
        MyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLoby()
    {
        string ipAddress = InputField.text;
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        connectButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        connectButton.interactable = true;

        gameObject.SetActive(false);
        landingPage.SetActive(false);
    }
    private void HandleClientDisconnected()
    {
        connectButton.interactable = true;
    }
}
