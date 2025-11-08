using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private IUnit unit;
    private Vector3 targetPosition, scale;
    private MapGrid grid;
    private GameObject visual;
    private Sprite gridVisual;

    public MoveCommand(IUnit unit, Vector3 targetPosition, MapGrid grid, Sprite gridVisual, Vector3 scale) {
        this.unit = unit;
        this.targetPosition = targetPosition;
        this.grid = grid;
        this.gridVisual = gridVisual;
        this.scale = scale;
    }

    public void Execute()
    {
        if (unit != null) {
            grid.ClearGridTaken(unit.GetPosition());
            grid.SetGridTaken(targetPosition);
            unit.Move(targetPosition);
        }

        UndoGridVisual();
    }

    public void PrepareGridData()
    {
        grid.ReserveGridTaken(targetPosition);
    }

    public void PrepareGridVisual()
    {
        if (gridVisual != null) {
            visual = new GameObject();
            visual.AddComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            visual.GetComponent<SpriteRenderer>().sprite = gridVisual;
            visual.GetComponent<SpriteRenderer>().sortingOrder = 2;
            visual.transform.position = targetPosition;
            visual.transform.localScale = scale;
        }
    }

    public void UndoGridData()
    {
        grid.ClearGridTaken(targetPosition);
    }

    public void UndoGridVisual()
    {
        if (visual != null) {
            GameObject.Destroy(visual);
        }
    }
}
