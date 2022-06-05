using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{   
    [SerializeField] private InputField nameInputField = null;
    [SerializeField] private Button continourButton = null;

    public static string DisplayName{ get; private set;}

    private const string playerPrefsNameKey = "PlayerName";

    private void Start() => GetSavedDisplayName();

    private void GetSavedDisplayName()
    {
        if(!PlayerPrefs.HasKey(playerPrefsNameKey)) { return; }

        string playerName = PlayerPrefs.GetString(playerPrefsNameKey);

        nameInputField.text = playerName;

        // SetPlayerName(playerName);
    }

    // public void SetPlayerName(string playerName)
    // {
    //     // continourButton.interactable = !string.IsNullOrEmpty(playerName);
    // }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(playerPrefsNameKey, DisplayName);
    }
}
