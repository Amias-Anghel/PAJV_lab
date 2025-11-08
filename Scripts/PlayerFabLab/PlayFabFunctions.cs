using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using NaughtyAttributes;

public class PlayFabFunctions : MonoBehaviour
{
    [SerializeField] private string customId;
    [SerializeField] private string email;
    [SerializeField] private string username;
    [SerializeField] private string password;
    [SerializeField] private string avatar;

    [Button]
    private void LoginWithCustomID() {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest(){
            CustomId = customId
        }, result => {
            Debug.LogFormat("Login CustomId {0} {1}", result.PlayFabId, result.LastLoginTime);
        }, error => {
            Debug.LogFormat("Login CustomId {0} {1}", error.ErrorMessage, error.Error);            
        });
    }

    [Button]
    private void LoginWithUsernameAndPassword() {
        var loginRequest = new LoginWithPlayFabRequest {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(
            loginRequest,
            result => {
                Debug.LogFormat("Login Successful! PlayFabId: {0}, LastLoginTime: {1}", result.PlayFabId, result.LastLoginTime);
            },
            error => {
                Debug.LogFormat("Login Failed: {0}, Error: {1}", error.ErrorMessage, error.Error);
            }
        );
    }


    [Button]
    private void UpdatePlayerData() {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
                {"AvatarName", avatar}
            }
        }, result => {
            Debug.LogFormat("Update Player Data");
        }, error => {
            Debug.LogFormat("Update Player Data {0} {1}", error.ErrorMessage, error.Error);            
        });
    }

    [Button]
    private void Register() {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest() {
            Email = email,
            Password = password,
            Username = username,
            DisplayName = username
        }, result => {
            Debug.LogFormat("Register {0}", result.PlayFabId);
        }, error => {
            Debug.LogFormat("Register {0} {1}", error.ErrorMessage, error.Error);            
        });
    }

    [Button]
    private void SavePlayerReadOnlyData() {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() {
            FunctionName = "savePlayer",
            FunctionParameter = new {
                characterName = username
            },
            GeneratePlayStreamEvent = true
        }, result => {
            Debug.LogFormat("Read Only Data");
        }, error => {
            Debug.LogFormat("Read Only Data {0} {1}", error.ErrorMessage, error.Error);            
        });
    }

    [Button]
    public void GetLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "Collectables", // Name of your statistic
            StartPosition = 0,             // Starting position (e.g., top of the leaderboard)
            MaxResultsCount = 10           // Number of entries to fetch
        },
        result =>
        {
            Debug.Log("Leaderboard fetched successfully!");
            foreach (var entry in result.Leaderboard)
            {
                Debug.Log($"Rank: {entry.Position + 1}, Player: {entry.DisplayName}, Score: {entry.StatValue}");
            }
        },
        error =>
        {
            Debug.LogError("Failed to fetch leaderboard: " + error.GenerateErrorReport());
        });
    }
}
