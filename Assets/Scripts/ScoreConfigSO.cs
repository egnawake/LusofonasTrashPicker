using UnityEngine;

[CreateAssetMenu(fileName="ScoreConfig", menuName="Score Config")]
public class ScoreConfigSO : ScriptableObject
{
    [field: SerializeField] public int Moved { get; private set; }
    [field: SerializeField] public int SkippedTurn { get; private set; }
    [field: SerializeField] public int CollectedTrash { get; private set; }
    [field: SerializeField] public int FailedToCollectTrash { get; private set; }

    public ScoreConfig ToScoreConfig()
    {
        return new ScoreConfig
        {
            Moved = this.Moved,
            SkippedTurn = this.SkippedTurn,
            CollectedTrash = this.CollectedTrash,
            FailedToCollectTrash = this.FailedToCollectTrash
        };
    }
}
