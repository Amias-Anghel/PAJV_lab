using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private List<AnimatorOverrideController> overriders;
    private Animator animator;

    private float speedBoost;

    void Start() {
        animator = GetComponent<Animator>();

        int randomIndex = Random.Range(0, overriders.Count - 1);

        animator.runtimeAnimatorController = overriders[randomIndex];
        speedBoost = randomIndex * 2 + 1;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        other.GetComponent<PlayerMovement>().TemporarySpeedBoost(speedBoost);
        Destroy(this.gameObject);
    }

}
