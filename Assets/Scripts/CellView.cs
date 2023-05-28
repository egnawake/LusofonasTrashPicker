using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private GameObject fogPrefab;

    private GameObject placedObject;

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
