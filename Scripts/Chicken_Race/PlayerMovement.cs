using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InputAction playerControls;
    [SerializeField] private float speed = 20f;
    private Vector2 moveDir = Vector2.zero;

    [SerializeField] private TMP_Text speedText;

    public bool canMove = true;
    private Animator animator;

    private Vector3 initialPoz;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        initialPoz = transform.position;
        Reset();
    }

    void Update()
    {
        if (canMove) {
            if (speed >= 70) {
                if (speedText != null) speedText.text = "MAX Speed: " + speed;
            } else {
                if (speedText != null) speedText.text = "Speed: " + speed;
            }
            Move();
        } else {
            if (speedText != null) speedText.text = "";
            rb.velocity = Vector2.zero;
            animator?.SetBool("walk", false);
        }
    }

    public void SetAnimator(AnimatorOverrideController _animator) {
        animator.runtimeAnimatorController = _animator;
    }

    private void Move() {
        moveDir = playerControls.ReadValue<Vector2>();
        rb.velocity = new Vector2(speed * moveDir.x, speed * moveDir.y);
        if (rb.velocity != Vector2.zero) {
            animator.SetBool("walk", true);
        } else {
            animator.SetBool("walk", false);
        }

        if (rb.velocity.x > 0) {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        } else {
            transform.rotation = Quaternion.identity;
        }
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    public void Reset() {
        speed = 20f; 
        transform.position = initialPoz;   
        StopAllCoroutines();
    }

    public void TemporarySpeedBoost(float boost) {
        StartCoroutine(SpeedBoost(boost));
    }

    IEnumerator SpeedBoost(float boost) {
        speed += boost;
        if (speed > 70) {
            speed = 70;
        }

        yield return new WaitForSecondsRealtime(5);

        speed -= boost;
        if (speed < 20) {
            speed = 20;
        }
    }
}
