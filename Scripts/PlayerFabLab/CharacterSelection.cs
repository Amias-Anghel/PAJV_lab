using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] PlayerMovement player1;
    
    [SerializeField] private GameObject instructions1;

    public List<AnimatorOverrideController> animators;

    private int index1 = 0;
    private bool ready1 = false, loggedin = false;

    void Start() {
        player1.SetAnimator(animators[0]);
    }

    void Update () {     
        if (loggedin) {
            CharacterSelect();
        }
    }

    public void SetLoggedIn() {
        loggedin = true;
    }

    private void CharacterSelect() {
        if (Input.GetKeyDown(KeyCode.D) && !ready1) {
            index1++;
            if (index1 >= animators.Count) {
                index1 = 0;
            }

            player1.SetAnimator(animators[index1]);
        }

        if (Input.GetKeyDown(KeyCode.A) && !ready1) {
            index1--;
            if (index1 < 0 ) {
                index1 = animators.Count - 1;
            }

            player1.SetAnimator(animators[index1]);
        }

        if (Input.GetKeyDown(KeyCode.W) && !ready1) {
            UpdatePlayerData(animators[index1].name, index1.ToString());
            ready1 = true;
            player1.canMove = true;
            instructions1.SetActive(false);
        }
    }

    private void UpdatePlayerData(string avatar, string avatarId) {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
                {"AvatarName", avatar},
                {"AvatarId", avatarId}
            }
        }, result => {
            Debug.LogFormat("Update Player Data");
        }, error => {
            Debug.LogFormat("Update Player Data {0} {1}", error.ErrorMessage, error.Error);            
        });
    }
}
