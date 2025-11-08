using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class UserScore : MonoBehaviour
{
    [SerializeField] private TMP_Text score_text;
    [SerializeField] private int score = 0;

    void Start() {
        score_text.text = "Score: " + score;
    }

    public void IncreaseScore() {
        score++;
        score_text.text = "Score: " + score;
    }

    void OnApplicationQuit() {
        Debug.Log("Application quit");
        SavePlayerReadOnlyData();
        SendLeaderboard();
    }

    private void SavePlayerReadOnlyData() {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() {
            FunctionName = "savePlayerScore",
            FunctionParameter = new {
                playerScore = score
            },
            GeneratePlayStreamEvent = true
        }, result => {
            Debug.LogFormat("Read Only Data");
        }, error => {
            Debug.LogFormat("Read Only Data {0} {1}", error.ErrorMessage, error.Error);            
        });

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() {
            FunctionName = "saveTotalTimeSpent",
            GeneratePlayStreamEvent = true
        }, result => {
            Debug.LogFormat("Read Only Data");
        }, error => {
            Debug.LogFormat("Read Only Data {0} {1}", error.ErrorMessage, error.Error);            
        });
    }

    private void SendLeaderboard() {
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest {
                Statistics = new List<StatisticUpdate> {
                    new StatisticUpdate {
                        StatisticName = "PlayerScore",
                        Value = score
                    }
                }
            }, result => {
                Debug.LogFormat("Successfull leaderboard update");
            }, error => {
                Debug.LogFormat("Leaderboard update {0} {1}", error.ErrorMessage, error.Error);
            });
    }
}
