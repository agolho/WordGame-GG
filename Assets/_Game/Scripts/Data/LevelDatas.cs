using UnityEngine;

namespace TileGame
{
    [CreateAssetMenu(fileName = "NewLevelJsonDatas", menuName = "ScriptableObjects/LevelJsonDatas", order = 1)]
    public class LevelDatas : ScriptableObject
    {
        public TextAsset[] levelDatas;

        public PuzzleData GetLevelData(int index)
        {
            if (index < levelDatas.Length)
                return GetPuzzleData(index);

            Debug.LogError("Level index out of bounds!");
            return GetPuzzleData(0);
        }

        private PuzzleData GetPuzzleData(int index)
        {
            return JsonUtility.FromJson<PuzzleData>(levelDatas[index].text);
        }
    }
}