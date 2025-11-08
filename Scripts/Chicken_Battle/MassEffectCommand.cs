using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassEffectCommand : ICommand
{
    private Vector3 targetPosition;
    private MapGrid grid;
    private float power;
    private int maxDistance;
    private Sprite gridVisual, gridPrevVisual;
    private List<GameObject> visuals;
    private List<Vector2> tiles;

    public MassEffectCommand(float power, int maxDistance, Vector3 targetPosition, MapGrid grid,Sprite gridPrevVisual, Sprite gridVisual) {
        this.targetPosition = targetPosition;
        this.grid = grid;
        this.power = power;
        this.maxDistance = maxDistance;
        this.gridVisual = gridVisual;
        this.gridPrevVisual = gridPrevVisual;

        tiles = grid.GetMassEffectInteractionTiles(targetPosition, maxDistance);
        visuals = new List<GameObject>();
    }

    public void Execute()
    {
        grid.RegisterMassDamageTile(targetPosition, power, maxDistance);

        if (visuals.Count == 0) {
            PrepareGridVisual();
        }

        foreach (GameObject visual in visuals) {
           visual.AddComponent<TileEffect>().StartAnimation(gridVisual);
        }
    }

    public void PrepareGridData()
    {
    }

    public void PrepareGridVisual()
    {
        if (gridVisual != null) {
            foreach (Vector2 tile in tiles) {
                GameObject visual = new GameObject();
                visual.AddComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
                visual.GetComponent<SpriteRenderer>().sprite = gridPrevVisual;
                visual.GetComponent<SpriteRenderer>().sortingOrder = 1;
                visual.transform.position = grid.GetWorldPositionCentered((int)tile.x, (int)tile.y);
                visual.transform.localScale = new Vector3(14, 14, 1);
                
                visuals.Add(visual);
            }
        }
    }

    public void UndoGridData()
    {
    }

    public void UndoGridVisual()
    {
        if (visuals.Count != 0) {
            foreach (GameObject visual in visuals) {
                GameObject.Destroy(visual);
            }
        }
    }
}
