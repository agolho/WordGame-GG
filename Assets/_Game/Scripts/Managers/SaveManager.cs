using System.IO;
using UnityEngine;

namespace TileGame
{
    public class SaveManager : MonoBehaviour, IInitializable
    {
       
        private LevelDatas _levelDatas;
        private ScoreManager _scoreManager;
        private LevelManager _levelManager;
        private UIManager _uiManager;
        
        private int[] _highScores;
        private int[] _levelLockedStates;

        public void Initialize(GameScriptsConfig config)
        {
            AssignScripts(config);

            CreateTemplateData();
            LoadGame();
        }

        private void AssignScripts(GameScriptsConfig config)
        {
            _levelDatas = config.LevelDatas;;
            _scoreManager = config.ScoreManager;
            _levelManager = config.LevelManager;
            _uiManager = config.UIManager;
        }

        private void CreateTemplateData()
        {
            _highScores = new int[_levelDatas.levelDatas.Length];
            _levelLockedStates = new int[_levelDatas.levelDatas.Length];
            
            for (var i = 0; i < _levelDatas.levelDatas.Length; i++)
            {
                _highScores[i] = 0;
                _levelLockedStates[i] = 0;
            }
            _levelLockedStates[0] = 1;
        }

        public void SaveGame()
        {
            var saveData = new SaveData
            {
                highScores = _highScores,
                levelLockedStates = _levelLockedStates
            };
            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString("SaveData", json);
            PlayerPrefs.Save();
        }

        private void LoadGame()
        {
            if (PlayerPrefs.HasKey("SaveData"))
            {
                var json = PlayerPrefs.GetString("SaveData");
                var saveData = JsonUtility.FromJson<SaveData>(json);
                _highScores = saveData.highScores;
                _levelLockedStates = saveData.levelLockedStates;
            }
            else
            {
                SaveGame();
            }
        }
        
        public void SetHighScore(int levelIndex, int score)
        {
            if (score > _highScores[levelIndex])
            {
                _highScores[levelIndex] = score;
                SaveGame();
            }
        }
        public void UnlockLevel(int levelIndex)
        {
            if (_levelLockedStates[levelIndex] == 1)
            {
                return;
            }
            _levelLockedStates[levelIndex] = 1;
            _uiManager.SetToUnlockLevel(levelIndex);
            SaveGame();
        }
        
        public void SaveScores()
        {
            var levelScore = _scoreManager.CalculateLevelScore();
            SetHighScore(_levelManager.GetCurrentLevelIndex(), levelScore);
            
            var levelToUnlock = _levelManager.GetCurrentLevelIndex() + 1;
            UnlockLevel(levelToUnlock);
        }
        
        public int GetHighScore(int levelIndex)
        {
            if(_highScores.Length > levelIndex)
                return _highScores[levelIndex];
            
            _highScores = new int[_levelDatas.levelDatas.Length];
            SaveGame();
            return 0;
        }
        
        public bool IsLevelLocked(int levelIndex)
        {
            if(_levelLockedStates.Length > levelIndex)
                return _levelLockedStates[levelIndex] == 0;
            
            _levelLockedStates = new int[_levelDatas.levelDatas.Length];
            SaveGame();
            return true;
        }

        private void ResetGame()
        {
            if (_levelDatas == null) return;
            _highScores = new int[_levelDatas.levelDatas.Length];
            SaveGame();
            Debug.Log("Game Reset");
        }

        [ContextMenu("Save Game")]
        public void SaveGameEditor()
        {
            SaveGame();
        }

        [ContextMenu("Load Game")]
        public void LoadGameEditor()
        {
            LoadGame();
        }

        [ContextMenu("Reset Game")]
        public void ResetGameEditor()
        {
            ResetGame();
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int[] highScores;
        public int[] levelLockedStates;
    }
}