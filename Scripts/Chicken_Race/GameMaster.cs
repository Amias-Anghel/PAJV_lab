using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private TMP_Text winText1;
    [SerializeField] private TMP_Text winText2;
    [SerializeField] private Button restartButt1;
    [SerializeField] private Button restartButt2;
    [SerializeField] private GameObject instructions1;
    [SerializeField] private GameObject instructions2;
    [SerializeField] private GameObject readySet1;
    [SerializeField] private GameObject readySet2;

    [SerializeField] PlayerMovement player1;
    [SerializeField] PlayerMovement player2;

    private int turnP1 = 0, turnP2 = 0;

    [SerializeField] PlayerProgress playerProgress1;
    [SerializeField] PlayerProgress playerProgress2;

    public List<AnimatorOverrideController> animators;

    private int index1 = 0, index2 = 0;
    private bool ready1 = false, ready2 = false;

    void Start() {
        player1.SetAnimator(animators[0]);
        player2.SetAnimator(animators[0]);

        RestartGame();
    }

    void Update () {
        CharacterSelection();
    }

    private void CharacterSelection() {
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
            ready1 = true;
            instructions1.SetActive(false);
            StartRace();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !ready2) {
            index2++;
            if (index2 >= animators.Count) {
                index2 = 0;
            }

            player2.SetAnimator(animators[index2]);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !ready2) {
            index2--;
            if (index2 < 0 ) {
                index2 = animators.Count - 1;
            }

            player2.SetAnimator(animators[index2]);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && !ready2) {
            ready2 = true;
            instructions2.SetActive(false);
            StartRace();
        }
    }

    private void OnTriggerEnter2D( Collider2D other) {
        if (other.CompareTag("Player")) {
            int turn = other.GetComponent<PlayerProgress>().turnNumber;
            if (turn == 3) {
                winText1.text = other.name + " WINS!";
                winText2.text = other.name + " WINS!";

                restartButt1.gameObject.SetActive(true);
                restartButt2.gameObject.SetActive(true);

                player1.canMove = false;
                player2.canMove = false;
            }

            if (other.name == "Player1" && turnP1 <= turn) {
                turnP1++;
                Checkpoint.SpawnBoosts?.Invoke();
            } else if (other.name == "Player2" && turnP2 <= turn) {
                turnP2++;
                Checkpoint.SpawnBoosts?.Invoke();
            }
        }
    }

    public void RestartGame() {
        readySet1.SetActive(false);
        readySet2.SetActive(false);

        player1.canMove = false;
        player1.canMove = false;

        winText1.text = "";
        winText2.text = "";

        turnP1 = 0;
        turnP2 = 0;

        restartButt1.gameObject.SetActive(false);
        restartButt2.gameObject.SetActive(false); 

        instructions1.SetActive(true);
        instructions2.SetActive(true);

        player1.Reset();
        player2.Reset();
        playerProgress1.Reset();
        playerProgress2.Reset();

        ready1 = false;
        ready2 = false; 
    }

    public void StartRace() {
        if (!ready1 || !ready2) 
            return;

        StartCoroutine(ReadySetGo());
    }

    IEnumerator ReadySetGo() {
        readySet1.SetActive(true);
        readySet2.SetActive(true);

        readySet1.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ready!";
        readySet2.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ready!";

        yield return new WaitForSecondsRealtime(2);

        readySet1.transform.GetChild(0).GetComponent<TMP_Text>().text = "Set!";
        readySet2.transform.GetChild(0).GetComponent<TMP_Text>().text = "Set!";

        yield return new WaitForSecondsRealtime(2);

        readySet1.transform.GetChild(0).GetComponent<TMP_Text>().text = "Go!";
        readySet2.transform.GetChild(0).GetComponent<TMP_Text>().text = "Go!";

        player1.canMove = true;
        player2.canMove = true;

        yield return new WaitForSecondsRealtime(1);

        readySet1.SetActive(false);
        readySet2.SetActive(false);
    }
}
