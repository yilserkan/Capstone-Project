using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseConnection : MonoBehaviour
{
    public static event Action loginWasSuccesful;
    public static event Action registerWasSuccesful;
    public static event Action<string> questionsRead;

    private void Start() {
        // StartCoroutine(GetUser());
        // StartCoroutine(Login("test","1234"));
        // StartCoroutine(RegisterUser("test2", "123"));
    }

    IEnumerator GetUser()
    {
        using(UnityWebRequest www = UnityWebRequest.Get("http://localhost/UnityBackendTutorial/GetUsers.php"))
        {
            yield return www.SendWebRequest();

            if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else{
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public static IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUsername", username);
        form.AddField("loginPassword", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityBackendTutorial/Login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                if(www.downloadHandler.text.Equals("True"))
                {
                    loginWasSuccesful?.Invoke();
                }
            }
        }
    }

    public static IEnumerator RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUsername", username);
        form.AddField("loginPassword", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityBackendTutorial/RegisterUser.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                if (www.downloadHandler.text.Equals("True"))
                {
                    registerWasSuccesful?.Invoke();
                }
            }
        }
    }

    public static IEnumerator GetQuestions()
    {
        Debug.Log("Heyooooooo");
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityBackendTutorial/GetQuestions.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                questionsRead?.Invoke(www.downloadHandler.text);
            }
        }
    }


}
