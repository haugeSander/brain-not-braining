using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 12;
    public int height = 10;
    public float tileSpacing = 1.1f;

    public Tile tilePrefab;
    private Tile[,] grid;

    void Start()
    {
        GenerateGrid();
        List<Vector2Int> path = GeneratePath();
        StartCoroutine(PreviewPath(path));
    }

    void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * tileSpacing, 0, y * tileSpacing);
                Tile tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                grid[x, y] = tile;
            }
        }
    }

    List<Vector2Int> GeneratePath()
    {
        List<Vector2Int> path = new List<Vector2Int>();

        int y = Random.Range(0, height);

        for (int x = 0; x < width; x++)
        {
            Vector2Int current = new Vector2Int(x, y);
            path.Add(current);
            grid[x, y].isCorrectTile = true;

            // Random vertical movement
            if (Random.value > 0.5f)
                y += Random.Range(-1, 2);

            y = Mathf.Clamp(y, 0, height - 1);
        }

        return path;
    }

    IEnumerator PreviewPath(List<Vector2Int> path)
    {
        // Show path
        foreach (var p in path)
            grid[p.x, p.y].SetPreview(true);

        yield return new WaitForSeconds(2f);

        // Hide path
        foreach (var p in path)
            grid[p.x, p.y].SetPreview(false);
    }
}


