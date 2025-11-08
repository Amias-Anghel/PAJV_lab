using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IUnit
{
    [SerializeField] private GameObject optionsMenuPrefab;
    [SerializeField] private int checkCode = 0;
    public bool massHeal = false; 
    [SerializeField] private Sprite healSprite, healPrevSprite;
    public bool massAttack = false; 
    [SerializeField] private Sprite attkSprite, attkPrevSprite;
    public int cooldown = 0;
    [SerializeField] private float specialPower = 0.1f;
    [SerializeField] private int maxDistance = 1;
    [SerializeField] private float attackPower = 0.2f;
    [SerializeField] private float health = 1f;
    private MapGrid grid; 
    private bool isSelected = false;
    private Vector3 target; 
    public bool canMoveToTarget = false;
    GameObject menu;

    [SerializeField] private Slider slider; 

    ICommand command;
 
    void Awake() {
        InterfaceControl.BroadcastMapGrid += PositionOnMap;
        InterfaceControl.CheckGridDamage += HandleCheckGridDamage; 
        InterfaceControl.SentCommands += SendCommand;

        slider.value = 1;
        health = 1f;
    }

    private void PositionOnMap(MapGrid grid) {
        this.grid = grid;

        for (int i = 0; i < grid.height; i++) {
            if (!grid.IsGridTaken(0, i)) {
                grid.SetGridTaken(0, i);
                transform.position = grid.GetWorldPositionCentered(0, i);
                break;
            }
        }
    }

    public void SetUnitStats(float attackPower, int maxDistance, float specialPower, bool massHeal) {
            this.maxDistance = maxDistance;
            this.attackPower = attackPower;
            this.massHeal = massHeal;
            this.specialPower = specialPower;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (SelectUnit()) {
                grid.HighlightTilesChecked(transform.position, maxDistance, checkCode);
                isSelected = true;
            } else {
                isSelected = false;
                grid.ClearHighlightsChecked(checkCode);
                if (menu != null && menu.transform.position != grid.GetWorldPositionCentered(MapGrid.GetWorldMousePosition())) {
                    menu.SetActive(false);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && isSelected) {
            target = grid.GetWorldPositionCentered(MapGrid.GetWorldMousePosition());
            if (grid.IsHighlighted(target)) {
                canMoveToTarget = !grid.IsGridTaken(target);
                if (menu != null) {
                    menu.SetActive(true);
                    menu.transform.position = target;
                } else {
                    menu = Instantiate(optionsMenuPrefab, target, Quaternion.identity);
                }
                
                menu.GetComponent<InteractionMenu>().ShowInteractionsMenu(this);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            SendCommand();
        } 
    }

    private void SendCommand() {
        if (command != null) {
            InterfaceControl.RegisterCommand?.Invoke(command);
            command = null;
        }
    }

    public void RegisterCommandMove() {
        command?.UndoGridVisual();
        command?.UndoGridData();
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        command = new MoveCommand(this, target, grid, sprite, transform.localScale);
        command.PrepareGridVisual();
        command.PrepareGridData();
        menu.SetActive(false);
    }

    public void RegisterCommandAttack() {
        command?.UndoGridVisual();
        command?.UndoGridData();
        command = new SimpleAttackCommand(-attackPower, target, grid, attkPrevSprite, attkSprite);
        command.PrepareGridVisual(); 
        command.PrepareGridData();
        menu.SetActive(false);
    }

    public void RegisterCommandMassAttack() {
        command?.UndoGridVisual();
        command?.UndoGridData();
        command = new MassEffectCommand(-specialPower, maxDistance, target, grid, attkPrevSprite, attkSprite);
        command.PrepareGridVisual();
        command.PrepareGridData();
        menu.SetActive(false);
    }

    public void RegisterCommandHeal() {
        command?.UndoGridVisual();
        command?.UndoGridData();
        command = new MassEffectCommand(specialPower, maxDistance, target, grid, healPrevSprite, healSprite);
        command.PrepareGridVisual();
        command.PrepareGridData();
        menu.SetActive(false);
    }

    private bool SelectUnit() {
        Vector3 click = MapGrid.GetWorldMousePosition();
        Vector3 position = grid.GetWorldPositionCentered(click);

        return position == transform.position;
    }

    private void HandleCheckGridDamage() {
        float power = grid.TakeTileEffectDamage(transform.position);
        ModifyLife(power);
    }

    public void ModifyLife(float power) { 
        health += power;

        if (health <= 0) {
            command.UndoGridVisual();
            command.UndoGridData();
            Destroy(gameObject);
        }

        if (health > 1) {
            health = 1;
        }

        slider.value = health;
    }

    public void Move(Vector3 position) {
        transform.position = position;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
}
