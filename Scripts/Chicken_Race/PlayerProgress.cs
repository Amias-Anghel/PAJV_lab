using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerProgress : MonoBehaviour
{
    [SerializeField] public int turnNumber = 0;
    [SerializeField] private int checkpoint = 0;
    [SerializeField] private int progress = 0;
    [SerializeField] private InputAction playerToCheckpoint;

    private Vector2 lastCheckpoint;

    [SerializeField] private TMP_Text turnText;

    void Start() {
        Reset();

        playerToCheckpoint.performed += ctx => TeleportToCheckpoint();
    }

    private void OnEnable() {
        playerToCheckpoint.Enable();
    }

    private void OnDisable() {
        playerToCheckpoint.Disable();
    }

    public void PassedCheckpoint(int checkpointID, Vector2 position) {
        lastCheckpoint = position;
        progress = checkpointID;

        if (checkpointID > checkpoint) {
            if (checkpointID >= 15 && checkpoint != 0) {
                turnNumber++;
                turnText.text = turnNumber + "/3";
            }
            checkpoint = checkpointID;
        } else if (checkpointID == 0) {
            checkpoint = 0;
        }


        Debug.Log(gameObject.name + " at checkpoint " + checkpoint);
    }

    public void TeleportToCheckpoint() {
        transform.position = lastCheckpoint;
    }

    public void Reset() {
        turnNumber = 0;
        turnText.text = turnNumber + "/3";
        checkpoint = 0;
        progress = 0;
    }
}
