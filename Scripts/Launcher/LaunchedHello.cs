using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LaunchedHello : MonoBehaviour
{
    [SerializeField] private TMP_Text helloText;
    void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs ();
            for (int i = 0; i < args.Length; i++) {
                Debug.Log ("ARG " + i + ": " + args [i]);
            }

        StartCoroutine(GetCurrentUser(args[2]));
    }

    IEnumerator GetCurrentUser(string sessionToken)
    {
        string url = "https://parseapi.back4app.com/users/me";

        UnityWebRequest uwr = new UnityWebRequest(url, "GET");
        uwr.SetRequestHeader("X-Parse-Application-Id", "1Q9zhJuNP9jdsrnsZ9aZGp9gW9rPh8OCniPRWj53");
        uwr.SetRequestHeader("X-Parse-REST-API-Key", "f5LI5xGgDZaB1U43105BWMjRyrQ2SAq2CFABfsSG");
        uwr.SetRequestHeader("X-Parse-Session-Token", sessionToken);

        uwr.downloadHandler = new DownloadHandlerBuffer();

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + uwr.downloadHandler.text);

            UserResponse userResponse = JsonUtility.FromJson<UserResponse>(uwr.downloadHandler.text);
            helloText.text = "Hello " + userResponse.username;
        }
        else
        {
            Debug.LogError($"Failed to fetch user: {uwr.error}");
        }
    }
}

[System.Serializable]
public class UserResponse
{
    public string username;
    public string myCustomKeyName;
    public string createdAt;
    public string updatedAt;
    public string objectId;
    public string sessionToken;
}
