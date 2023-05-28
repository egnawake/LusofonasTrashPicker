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

    [SerializeField] private GameObject cellObjectPrefab;

    private TrashPickerGame game;
    private GameObject[,] cellObjects;

    private void Start()
    {
        // Instantiate game
        game = new TrashPickerGame(rows, cols, maxTurns, trashSpawnChance);

        cellObjects = new GameObject[rows, cols];

        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                GameObject cellObject = Instantiate(cellObjectPrefab, transform);

                cellObject.name = $"CellObject({i}, {j})";
                cellObject.transform.position = PositionCellObject(i, j);

                cellObjects[i, j] = cellObject;
            }
        }
    }

    private Vector3 PositionCellObject(int row, int col)
    {
        Vector3 position = new Vector3(col * gridSpacing, 0,
            -(row * gridSpacing));

        // Offset to center
        float rowLength = gridSpacing * (game.Cols - 1);
        float colLength = gridSpacing * (game.Rows - 1);
        Vector3 offset = new Vector3(-rowLength / 2, 0, colLength / 2);

        return position + offset;
    }
}
