using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class EggSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef eggPrefab = NetworkPrefabRef.Empty;

    private float _screenBoundaryX = 0.0f;
    private float _screenBoundaryY = 0.0f;

    public static Action SpawnNewEgg;

    public void StartEggSpawner()
    {
        SpawnNewEgg += SpawnEgg;
        if (!Object.HasStateAuthority) return;

        _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
        _screenBoundaryY = Camera.main.orthographicSize;

        for (int i = 0; i < 5; i++) {
            SpawnEgg();
        }
    }

    private void SpawnEgg()
    {
        Vector3 position;
        position.x = UnityEngine.Random.Range(-_screenBoundaryX, _screenBoundaryX);
        position.y = UnityEngine.Random.Range(-_screenBoundaryY, _screenBoundaryY);
        position.z = 0;

        position -= position.normalized * 0.2f;
        if (Runner != null) {
            Runner.Spawn(eggPrefab, position, Quaternion.identity);
        }
    }
}
