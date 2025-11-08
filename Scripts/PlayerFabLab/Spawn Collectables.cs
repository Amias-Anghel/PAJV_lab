using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectables : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private Transform player;

    public static Action CollectCollectable;

    void Start() {
        CollectCollectable += SpawnCollectable;

        for (int i = 0; i < 20; i++) {
            SpawnCollectable();
        }
    }

    private void SpawnCollectable() {
        int distx = UnityEngine.Random.Range(-10, 10);
        int disty = UnityEngine.Random.Range(-10, 10);
        Vector2 position = (Vector2)player.position + new Vector2(distx, disty);
        Instantiate(prefab, position, Quaternion.identity);
    }
}
