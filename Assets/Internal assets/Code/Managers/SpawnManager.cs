using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform gridTransform;
    [SerializeField] Baloon gameElementPrefab;

    void Start()
    {
        GameManager.Instance.Grid = new Baloon[Settings.Instance.Columns, Settings.Instance.Rows];
        StartFillGrid();
    }

    private void StartFillGrid()
    {
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                int RandomColor = GetNextColotWithoutMatchs(i, j);
                InstantiateElement(new Vector2Int(i,j), RandomColor);
            }
        }
    }
    private void InstantiateElement(Vector2Int gridCoordinate, int color)
    {
        Vector2 elementPos = new Vector2(gridCoordinate.x, gridCoordinate.y);
        Baloon gameElement = Instantiate(gameElementPrefab, elementPos, Quaternion.identity, gridTransform);
        gameElement.Init(gridCoordinate.x, gridCoordinate.y, color);
        GameManager.Instance.Grid[gridCoordinate.x, gridCoordinate.y] = gameElement;
    }
    private int GetNextColotWithoutMatchs(int column, int row)
    {
        int RandomColor = Random.Range(0, Settings.Instance.colorArray.Length);
        int maxIterations = 0;

        while (CheckIsElemenMatched(column, row, RandomColor) && maxIterations < 100)
        {
            RandomColor = Random.Range(0, Settings.Instance.colorArray.Length);
            maxIterations++;
        }
        return RandomColor;
    }
    public void RefillGrid()
    {
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                if (GameManager.Instance.Grid[i, j] == null)
                {
                    int RandomColor = Random.Range(0, Settings.Instance.colorArray.Length);
                    InstantiateElement(new Vector2Int(i, j), RandomColor);
                }
            }
        }
    }

    private bool CheckIsElemenMatched(int column, int row, int color)
    {
        if (column > 1 && row > 1)
        {
            if (GameManager.Instance.Grid[column - 1, row].ColorType == color && GameManager.Instance.Grid[column - 2, row].ColorType == color)
            {
                return true;
            }
            if (GameManager.Instance.Grid[column, row - 1].ColorType == color && GameManager.Instance.Grid[column, row - 2].ColorType == color)
            {
                return true;
            }

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (GameManager.Instance.Grid[column, row - 1].ColorType == color && GameManager.Instance.Grid[column, row - 2].ColorType == color)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (GameManager.Instance.Grid[column - 1, row].ColorType == color && GameManager.Instance.Grid[column - 2, row].ColorType == color)
                {
                    return true;
                }
            }
        }

        return false;
    }
}