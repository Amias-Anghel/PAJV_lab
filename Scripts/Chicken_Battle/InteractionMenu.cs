using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMenu : MonoBehaviour
{
    private GameObject moveButton;
    private GameObject attkButton;
    private GameObject mattkButton;
    private GameObject mhealButton;

    private Unit unit;

    void Awake() {
        moveButton = transform.GetChild(0).gameObject;
        moveButton.GetComponent<Button>().onClick.AddListener(OnClickMove);
        attkButton = transform.GetChild(1).gameObject;
        attkButton.GetComponent<Button>().onClick.AddListener(OnClickAttk);
        mattkButton = transform.GetChild(2).gameObject;
        mattkButton.GetComponent<Button>().onClick.AddListener(OnClickMassAttk);
        mhealButton = transform.GetChild(3).gameObject;
        mhealButton.GetComponent<Button>().onClick.AddListener(OnClickHeal);
    }

    public void ShowInteractionsMenu(Unit unit) {
        this.unit = unit;

        moveButton.SetActive(unit.canMoveToTarget);
        mattkButton.SetActive(unit.massAttack);
        mhealButton.SetActive(unit.massHeal);

        if (unit.cooldown > 0) {
            mattkButton.GetComponent<Button>().enabled = false;
            mhealButton.GetComponent<Button>().enabled = false;
        }
    }

    private void OnClickMove() {
        unit.RegisterCommandMove();
    }
    private void OnClickAttk() {
        unit.RegisterCommandAttack();
    }
    private void OnClickMassAttk() {
        unit.RegisterCommandMassAttack();
    }
    private void OnClickHeal() {
        unit.RegisterCommandHeal();
    }
}
