using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class LogInForm : MonoBehaviour
{
    private string username, password;
    private CharacterSelection characterSelection;
    [SerializeField] private GameObject loginForm;

    void Start() {
        characterSelection = GetComponent<CharacterSelection>();
    }

    public void UpdateUsername(string value) {
        username = value;
    }

    public void UpdatePassword(string value) {
        password = value;
    }

    public void LoginWithUsernameAndPassword() {
        var loginRequest = new LoginWithPlayFabRequest {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(
            loginRequest,
            result => {
                Debug.LogFormat("Login Successful! PlayFabId: {0}, LastLoginTime: {1}", result.PlayFabId, result.LastLoginTime);
                characterSelection.SetLoggedIn();
                loginForm.SetActive(false);
            },
            error => {
                Debug.LogFormat("Login Failed: {0}, Error: {1}", error.ErrorMessage, error.Error);
            }
        );
    }
}
