using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private List<AnimatorOverrideController> overriders;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();

        int randomIndex = Random.Range(0, overriders.Count - 1);

        animator.runtimeAnimatorController = overriders[randomIndex];
    }

    private void OnTriggerEnter2D(Collider2D other) {
        other.GetComponent<UserScore>().IncreaseScore();
        SpawnCollectables.CollectCollectable?.Invoke();
        Destroy(this.gameObject);
    }

}
