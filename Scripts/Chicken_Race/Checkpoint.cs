using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointID;
    [SerializeField] private GameObject speedBoost;

    public static Action SpawnBoosts;

    void Start() {
        SpawnBoosts += HandleSpawnBoosts;
    }

    private void HandleSpawnBoosts()
    {
        int nrOfBoosts = UnityEngine.Random.Range(0, 2);
        float offsetX = UnityEngine.Random.Range(-2, 2);
        float offsetY = UnityEngine.Random.Range(-2, 2);

        for (int i = 0; i < nrOfBoosts; i++) {
            GameObject boost = GameObject.Instantiate(speedBoost, transform.position + new Vector3 (offsetX, offsetY, 0), Quaternion.identity);
            boost.transform.SetParent(this.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        other.GetComponent<PlayerProgress>()?.PassedCheckpoint(checkpointID, transform.position);
    }
}
