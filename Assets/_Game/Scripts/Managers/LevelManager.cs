using System.Collections;
using UnityEngine;

namespace TileGame
{
    public class LevelManager : MonoBehaviour, IInitializable
    {
        [Header("Managers & Components")]
        private TileManager _tileManager;
        private UIManager _uiManager;
        private ScoreManager _scoreManager;
        private SaveManager _saveManager;
        private AnalyticsManager _analyticsManager;
        
        private int _activeLevelIndex;
        private LevelDatas _levelDatas;
        private PuzzleData _currentPuzzleData;
        
        public void Initialize(GameScriptsConfig config)
        {
            AssignScripts(config);
            InitializeComponent();
        }
        private void AssignScripts(GameScriptsConfig config)
        {
            _tileManager = config.TileManager;
            _uiManager = config.UIManager;
            _levelDatas = config.LevelDatas;
            _scoreManager = config.ScoreManager;
            _saveManager = config.SaveManager;
            _analyticsManager = config.AnalyticsManager;
        }
        private void InitializeComponent()
        {
            AssignActiveLevelIndex();
        }
        
        public void LoadLevel(int levelIndex)
        {
            _activeLevelIndex = levelIndex;
            _currentPuzzleData = _levelDatas.GetLevelData(levelIndex);
            StartCoroutine(LoadLevelTiles());
            _uiManager.SwitchToGameplayScreen();
            _analyticsManager.LogEvent(_activeLevelIndex, LogEventTypes.LevelStarted);
        }

        private IEnumerator LoadLevelTiles()
        {
            yield return new WaitForSeconds(1); // Wait for the level selection panel to animate out
            _tileManager.LoadLevel(_currentPuzzleData);
        }

        private void AssignActiveLevelIndex()
        {
            var levelIndex = PlayerPrefs.GetInt("PlayerLevel", 0);
            _activeLevelIndex = levelIndex;
        }

        #region Functions
        public void CompleteLevel()
        {
            StartCoroutine(CompleteLevelRoutine());
            _analyticsManager.LogEvent(_activeLevelIndex, LogEventTypes.LevelCompleted);
            _saveManager.SaveScores();
        }

        private IEnumerator CompleteLevelRoutine()
        {
            yield return new WaitForSeconds(1);  // Wait for the last tile to finish animating
            _uiManager.SwitchToLevelEndScreen();
            _scoreManager.ResetScore();
        }

        #endregion
        
        public int GetCurrentLevelIndex()
        {
            return _activeLevelIndex;
        }
    }
}
