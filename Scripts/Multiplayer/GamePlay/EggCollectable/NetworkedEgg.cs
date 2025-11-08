using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class NetworkedEgg : NetworkBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer = null;

    [Networked] private NetworkBool wasCollected { get; set; }

    public override void Spawned()
    {
        wasCollected = false;
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    public void CollectEgg(PlayerRef player) {
        if (Object == null) return;
        if (!Object.HasStateAuthority) return;
        if (wasCollected) return;

        wasCollected = true;
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject)) {
            playerNetworkObject.GetComponent<PlayerDataNetworked>().AddToScore(1);
        }
    }

    public override void Render()
    {
        if (wasCollected)
        {
            EggSpawner.SpawnNewEgg?.Invoke();
            Runner.Despawn(Object);
        }
    }
}
