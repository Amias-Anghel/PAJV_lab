using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceControl : MonoBehaviour
{
    [SerializeField] GameObject tilePreset;
    private MapGrid grid;
    private Stack<ICommand> commands;

    public static Action<MapGrid> BroadcastMapGrid;
    public static Action<ICommand> RegisterCommand;
    public static Action CheckGridDamage;
    public static Action SentCommands;


    [SerializeField] TMP_Text timerText;

    int width, height;
    [SerializeField] private float cellsize = 10f; 

    void Awake()
    {
        SetupMap();
        commands = new Stack<ICommand>();
        RegisterCommand += RegisterCommandToStack;
    }

    private void SetupMap() {
        Vector3 cornerDownLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector3 cornerUpRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelRect.width, Camera.main.pixelRect.height, 10));
        width = Mathf.FloorToInt(cornerUpRight.x / (cellsize/2));
        height = Mathf.FloorToInt(cornerUpRight.y / (cellsize/2));
        grid = new MapGrid(width, height, cellsize, cornerDownLeft, tilePreset);

        BroadcastMapGrid?.Invoke(grid);  

        StartCoroutine(Timer());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ExecuteCommands();
        }
    }

    private void RegisterCommandToStack(ICommand command) {
        commands.Push(command);
    }

    public void ExecuteCommands() {
        SentCommands?.Invoke();
        
        StopAllCoroutines();

        grid.ClearDamageTiles(); // reset damage tiles

        while (commands.Count > 0)
        {
            ICommand activeCommand = commands.Pop();
            activeCommand.Execute();
        }

        CheckGridDamage?.Invoke();

        StartCoroutine(Timer());
    }

    private IEnumerator Timer() {
        int secs = 20;
        timerText.text = "Time left: " + secs;

        while (secs > 0) {
            yield return new WaitForSecondsRealtime(1);
            secs--; 
            timerText.text = "Time left: " + secs;
        }

        ExecuteCommands();
    }

    

}
