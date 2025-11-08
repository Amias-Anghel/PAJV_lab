using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IUnit
{
    public bool massHeal = false; 
    [SerializeField] private Sprite healSprite, healPrevSprite;
    public bool massAttack = false; 
    [SerializeField] private Sprite attkSprite, attkPrevSprite;

    public int cooldown = 0;
    [SerializeField] private float specialPower = 1f;
    [SerializeField] private int maxDistance = 1;
    [SerializeField] private float attackPower = 2f;
    [SerializeField] private float health = 10f;
    private MapGrid grid; 
    [SerializeField] private Slider slider; 

    private ICommand command;

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
            if (!grid.IsGridTaken(grid.width  - 1, i)) {
                grid.SetGridTaken(grid.width  - 1, i);
                transform.position = grid.GetWorldPositionCentered(grid.width  - 1, i);
                break;
            }
        }

        command = GenerateCommand();
    }

    public void SetUnitStats(float attackPower, int maxDistance, float specialPower, bool massHeal) {
            this.maxDistance = maxDistance;
            this.attackPower = attackPower;
            this.massHeal = massHeal;
            this.specialPower = specialPower;
    }

    void Update() {
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

    private ICommand GenerateCommand() {
        /* 0 - move, 1 - attk, 2 - mattk, 3 -mheal */
        int option = Random.Range(0, 4);
        Vector2 target = GetRandomPosition();
        // default move
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        ICommand command = new MoveCommand(this, target, grid, sprite, transform.localScale);
        command.PrepareGridData();
        command.PrepareGridVisual();

        switch (option) {
            case 1:
                command.UndoGridData();
                command.UndoGridVisual();
                command = new SimpleAttackCommand(-attackPower, target, grid, attkPrevSprite, attkSprite);
                command.PrepareGridVisual();
                break;
            case 2:
                if (massAttack) {
                    command.UndoGridData();
                    command.UndoGridVisual();
                    command = new MassEffectCommand(-specialPower, maxDistance, target, grid, attkPrevSprite, attkSprite);
                    command.PrepareGridVisual();
                }
                break;
            case 3:
                if (massHeal) {
                    command.UndoGridData();
                    command.UndoGridVisual();
                    command = new MassEffectCommand(specialPower, maxDistance, target, grid, healPrevSprite, healSprite);
                    command.PrepareGridVisual();
                }
                break;
        }

        return command;
    }

    private Vector3 GetRandomPosition() {
        List<Vector2> posibilities = grid.GetInteractionTiles(transform.position, maxDistance);

        for (int i = 0; i < posibilities.Count; i++) {
            if (!grid.IsGridTaken((int)posibilities[i].x, (int)posibilities[i].y)) {
                return grid.GetWorldPositionCentered((int)posibilities[i].x, (int)posibilities[i].y);
            }
        }

        return transform.position;
    }

    private void HandleCheckGridDamage() {
        float power = grid.TakeTileEffectDamage(transform.position);
        ModifyLife(power);

        command = GenerateCommand();
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
