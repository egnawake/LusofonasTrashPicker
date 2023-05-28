using UnityEngine;

public class TrashPickerBehaviour : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Number of rows in the grid.")]
    private int rows = 5;

    [SerializeField]
    [Tooltip("Number of columns in the grid.")]
    private int cols = 5;

    [SerializeField]
    [Tooltip("Amount of moves the robot can make.")]
    private int maxTurns = 20;

    [SerializeField]
    [Tooltip("Probability that a cell will have trash on it.")]
    [Range(0, 1f)]
    private double trashSpawnChance = 0.4;

    [SerializeField]
    [Tooltip("Distance between cell objects.")]
    private float gridSpacing = 3f;

    [SerializeField] private CellView cellPrefab;

    private TrashPickerGame game;
    private CellView[,] cellObjects;

    private void Start()
    {
        // Instantiate game
        game = new TrashPickerGame(rows, cols, maxTurns, trashSpawnChance);

        cellObjects = new CellView[rows, cols];

        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                CellView cellObject = Instantiate(cellPrefab, transform);

                cellObject.name = $"CellObject({i}, {j})";
                cellObject.transform.position = CellToWorldPosition(i, j);

                cellObjects[i, j] = cellObject;
            }
        }

        UpdateView();
    }

    private Vector3 CellToWorldPosition(int row, int col)
    {
        Vector3 position = new Vector3(col * gridSpacing, 0,
            -(row * gridSpacing));

        // Offset to center
        float rowLength = gridSpacing * (game.Cols - 1);
        float colLength = gridSpacing * (game.Rows - 1);
        Vector3 offset = new Vector3(-rowLength / 2, 0, colLength / 2);

        return position + offset;
    }

    private void UpdateView()
    {
        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                cellObjects[i, j].SetState(game.CellAt(new Position(i, j)));
            }
        }
    }
}
