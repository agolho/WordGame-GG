using UnityEngine;

namespace TileGame
{
    /// <summary>
    /// Struct to store the move record of a letter tile
    /// Used to undo moves in WordFormArea
    /// </summary>
public class MoveRecord
{
    public readonly LetterTile LetterTile;
    public readonly Transform OriginalParent;
    public readonly Vector3 OriginalPosition;
    public readonly LetterTile[] Children;
    public readonly float MoveDuration;
    public MoveRecord(LetterTile letterTile, Transform originalParent, Vector3 originalPosition, float moveDuration)
    {
        LetterTile = letterTile;
        OriginalParent = originalParent;
        OriginalPosition = originalPosition;
        Children = letterTile.GetChildren();
        MoveDuration = moveDuration;
    }
}

}

