using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttackCommand : ICommand
{
    private Vector3 targetPosition;
    private MapGrid grid;
    private float damage;
    private Sprite gridVisual, gridPrevVisual;
    private GameObject visual = null;

    public SimpleAttackCommand(float damage, Vector3 targetPosition, MapGrid grid,Sprite gridPrevVisual, Sprite gridVisual) {
        this.targetPosition = targetPosition;
        this.grid = grid;
        this.damage = damage;
        this.gridVisual = gridVisual;
        this.gridPrevVisual = gridPrevVisual;
    }

    public void Execute()
    {
        grid.RegisterDamageTile(targetPosition, damage);
        visual.AddComponent<TileEffect>().StartAnimation(gridVisual);
    }

    public void PrepareGridData()
    {
    }

    public void PrepareGridVisual()
    {
        if (gridVisual != null) {
            visual = new GameObject();
            visual.AddComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
            visual.GetComponent<SpriteRenderer>().sprite = gridPrevVisual;
            visual.GetComponent<SpriteRenderer>().sortingOrder = 1;
            visual.transform.position = targetPosition;
            visual.transform.localScale = new Vector3(14, 14, 1);
        }
    }

    public void UndoGridData()
    {
    }

    public void UndoGridVisual()
    {
        if (visual != null) {
            GameObject.Destroy(visual);
        }
    }
}
