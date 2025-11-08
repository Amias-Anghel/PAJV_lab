using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGrid
{
    public int height;
    public int width;
    private float cellsize;
    Vector3 originPosition;
    private int[,] gridTaken; // 1 occupies, 0 free, 2 to be occupied
    private GameObject[,] tileArray;
    private GameObject prefab;
    private GameObject gridParent;

    private List<SpriteRenderer> highlighted;
    private int highlightCheckCode = -1; // code to check highlight for specific unit

    private float[,] damageTiles; // tiles affected by attacks / heal: x,y = pos, z = power

    public MapGrid(int width, int height, float cellsize, Vector3 originPosition, GameObject prefab) {
        this.height = height;
        this.width = width;
        this.cellsize = cellsize;
        this.originPosition = originPosition;
        this.prefab = prefab;
        
        gridTaken = new int[width, height];
        tileArray = new GameObject[width, height];
        highlighted = new();
        damageTiles = new float[width, height];

        gridParent = new GameObject("Map Grid");

        for (int x = 0; x < tileArray.GetLength(0); x++) {
            for (int y = 0; y < tileArray.GetLength(1); y++) {
                tileArray[x, y] = CreateTile(GetWorldPositionCentered(x, y));
                tileArray[x, y].transform.SetParent(gridParent.transform);
            }
        }
    }

    public static Vector3 GetWorldMousePosition() {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0;
        return vec;
    }

    private bool IsInGrid(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellsize + originPosition;
    }

    public Vector3 GetWorldPositionCentered(int x, int y) {
        return GetWorldPosition(x, y) + new Vector3(cellsize, cellsize) * 0.5f;;
    }

    public Vector3 GetWorldPositionCentered(Vector3 position) {
        int x, y;
        GetXY(position, out x, out y);

        return GetWorldPosition(x, y) + new Vector3(cellsize, cellsize) * 0.5f;;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellsize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellsize);
    }

    private GameObject CreateTile(Vector3 localPosition) {
        GameObject tile = GameObject.Instantiate(prefab, localPosition, Quaternion.identity);
        tile.transform.localScale = new Vector3(cellsize / 2.5f, cellsize / 2.5f, 1);
        return tile;
    }

    public void SetGridTaken(int x, int y) {
        if (IsInGrid(x, y)) {
            gridTaken[x, y] = 1;
        }
    }

    public void SetGridTaken(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridTaken(x, y);
    }

    public void ReserveGridTaken(int x, int y) {
        if (IsInGrid(x, y)) {
            gridTaken[x, y] = 2;
        }
    }

    public void ReserveGridTaken(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        ReserveGridTaken(x, y);
    }

    public void ClearGridTaken(int x, int y) {
        if (IsInGrid(x, y)) {
            gridTaken[x, y] = 0;
        }
    }

    public void ClearGridTaken(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        ClearGridTaken(x, y);
    }

    public bool IsGridTaken(int x, int y) {
        if (IsInGrid(x, y)) {
            return gridTaken[x, y] == 1 || gridTaken[x, y] == 2;
        }

        return true;
    }

    public bool IsGridTaken(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return IsGridTaken(x, y);
    }

    public void HighlightTile(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);

        ClearHighlights();

        if (IsInGrid(x, y)) {  
            SpriteRenderer spriteRenderer = tileArray[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            highlighted.Add(spriteRenderer);
        }
    }

    public void HighlightTilesChecked(Vector3 worldPosition, int maxDistance, int checkCode) {
        ClearHighlights();
        highlightCheckCode = checkCode;
        
        int x,y;
        GetXY(worldPosition, out x, out y);

        if (!IsInGrid(x, y)) {
            return;
        }

        List<Vector2> highlights = GetInteractionTiles(x, y, maxDistance);

        if (highlights != null) {
            foreach(Vector2 v in highlights) {
                SpriteRenderer spriteRenderer = tileArray[(int)v.x, (int)v.y].GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                highlighted.Add(spriteRenderer);
            }
        }
    }

    public void HighlightTiles(Vector3 worldPosition, int maxDistance) {
        ClearHighlights();

        int x,y;
        GetXY(worldPosition, out x, out y);

        if (!IsInGrid(x, y)) {
            return;
        }

        List<Vector2> highlights = GetInteractionTiles(x, y, maxDistance);

        if (highlights != null) {
            foreach(Vector2 v in highlights) {
                SpriteRenderer spriteRenderer = tileArray[(int)v.x, (int)v.y].GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                highlighted.Add(spriteRenderer);
            }
        }
    }

    public bool IsHighlighted(Vector3 worldPosition) {
        int x,y;
        GetXY(worldPosition, out x, out y);

        if (IsInGrid(x, y) && highlighted.Contains(tileArray[x, y].GetComponent<SpriteRenderer>())) {
            return true;
        }

        return false;
    }

    public void ClearHighlights() {
        foreach (SpriteRenderer r in highlighted) {
            r.color = Color.white;
        }

        highlighted.Clear();
    }

    public void ClearHighlightsChecked(int checkCode) {
        if (checkCode != highlightCheckCode)
            return;

        ClearHighlights();
    }

    public List<Vector2> GetInteractionTiles(int x, int y, int maxDistance) {
        List<Vector2> posibilities = new();

        for (int i = maxDistance; i >= 0; i--) {
            if (x + i < width && y + i < height) {
                posibilities.Add(new Vector2(x + i, y + i));
            }

            if (x + i < width) {
                posibilities.Add(new Vector2(x + i, y));
            }

            if (y + i < height) {
                posibilities.Add(new Vector2(x, y + i));
            }

            if (x - i >= 0 && y - i >= 0) {
                posibilities.Add(new Vector2(x - i, y - i));
            }

            if (x - i >= 0) {
                posibilities.Add(new Vector2(x - i, y));
            }

            if (y - i >= 0) {
                posibilities.Add(new Vector2(x, y - i));
            }

            if (x - i >= 0 && y + i < height) {
                posibilities.Add(new Vector2(x - i, y + i));
            }

            if (x + i < width && y - i >= 0) {
                posibilities.Add(new Vector2(x + i, y - i));
            }
        }

        return posibilities;
    }

    public List<Vector2> GetInteractionTiles(Vector3 worldPosition, int maxDistance) {
        int x,y;
        GetXY(worldPosition, out x, out y);

        if (!IsInGrid(x, y)) {
            return null;
        }

        return GetInteractionTiles(x, y, maxDistance);
    }
    
    public List<Vector2> GetMassEffectInteractionTiles(int x, int y, int maxDistance) {
        List<Vector2> effect = new();

        int i = (x - maxDistance < 0) ? 0 : x - maxDistance; 
        int j = (y - maxDistance < 0) ? 0 : y - maxDistance;
        int limiti = (x + maxDistance >= width) ? width - 1 : x + maxDistance;
        int limitj = (y + maxDistance >= height) ? height - 1 : y + maxDistance;
    
        for (; i <= limiti; i++) {
            for(int jj = j; jj <= limitj; jj++) {
                effect.Add(new Vector2(i, jj));
            }
        }

        return effect;
    }

    public List<Vector2> GetMassEffectInteractionTiles(Vector3 worldPosition, int maxDistance) {
        int x,y;
        GetXY(worldPosition, out x, out y);

        if (!IsInGrid(x, y)) {
            return null;
        }

        return GetMassEffectInteractionTiles(x, y, maxDistance);
    }

    public void RegisterDamageTile(int x, int y, float power) {
        damageTiles[x, y] += power; 
    }

    public void RegisterDamageTile(Vector3 worldPosition, float power) {
        int x, y;
        GetXY(worldPosition, out x, out y);

        RegisterDamageTile(x, y, power);
    }

    public void RegisterMassDamageTile(Vector3 worldPosition, float power, int maxDistance) {
        List<Vector2> positions = GetMassEffectInteractionTiles(worldPosition, maxDistance);

        foreach (Vector2 pos in positions) {
            RegisterDamageTile((int)pos.x, (int)pos.y, power);
        }
    }

    public void ClearDamageTiles() {
        damageTiles = new float[width, height];
    }

    public float TakeTileEffectDamage(int x, int y) {
        return damageTiles[x, y];
    }

    public float TakeTileEffectDamage(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);

        return TakeTileEffectDamage(x, y);
    }
}
