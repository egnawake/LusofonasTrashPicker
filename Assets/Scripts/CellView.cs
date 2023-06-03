using UnityEngine;

/// <summary>
/// Cell renderer object. Shows what state a cell is in.
/// </summary>
public class CellView : MonoBehaviour
{
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private GameObject fogPrefab;

    private GameObject placedObject;

    /// <summary>
    /// Update contents of cell.
    /// </summary>
    ///
    /// <param name="state">
    /// The cell state to be shown.
    /// </param>
    public void SetState(CellState state)
    {
        if (placedObject != null)
            Destroy(placedObject);

        if (state == CellState.Empty)
            return;

        if (state == CellState.Trash)
            placedObject = Instantiate(trashPrefab, transform);
        else if (state == CellState.Hidden)
            placedObject = Instantiate(fogPrefab, transform);
    }
}
