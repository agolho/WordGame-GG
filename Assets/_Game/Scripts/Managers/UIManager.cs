using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame
{
    public class UIManager :  MonoBehaviour, IInitializable
    {
        [Header("Level Selection Screen Elements")] 
        [SerializeField] private Button levelsButton;

        [SerializeField] private GameObject levelSelectionPanel;
        [SerializeField] private Transform contentPanel;
        [SerializeField] private ScrollRect scrollRect;
        
        [Header("Gameplay Screen Elements")]
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI wordScoreText;
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private Button undoButton;

        [Header("Level End Screen Elements")]
        [SerializeField] private GameObject levelEndPanel;
        [SerializeField] private Button toLevelScreenButton;
        [SerializeField] private GameObject highScorePanel;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI levelScoreText;
        
        [Header("References")]
        private LevelManager _levelManager;
        private SaveManager _saveManager;
        private ScoreManager _scoreManager;
        private SentWordsDisplay _sentWordsDisplay;
        private LevelDatas _levelDatas;
        private AnalyticsManager _analyticsManager;
        
        [Header("Prefabs")]
        [SerializeField] private LevelSelector levelSelectorPrefab;
        
        
        private readonly List<LevelSelector> _levelSelectors = new List<LevelSelector>();
        private int _unlockAnimationIndex = -1;
        
        public void Initialize(GameScriptsConfig config)
        {
            AssignScripts(config);
            AddButtonListeners();
        }

        private void AssignScripts(GameScriptsConfig config)
        {
            _levelManager = config.LevelManager;
            _levelDatas = config.LevelDatas;
            _sentWordsDisplay = config.SentWordsDisplay;
            _saveManager = config.SaveManager;
            _scoreManager = config.ScoreManager;
            _analyticsManager = config.AnalyticsManager;
        }
        private void AddButtonListeners()
        {
            levelsButton.onClick.AddListener(ShowLevelSelectionPanel);
            toLevelScreenButton.onClick.AddListener(SwitchToLevelSelectionScreen);
        }
        
        public void SetToUnlockLevel(int levelIndex)
        {
            _unlockAnimationIndex = levelIndex;
        }
        
        private void ShowLevelSelectionPanel()
        {
            PopulateLevelSelectionPanel();
            SetPopupPanel(true);
            AnimateLevelSelectionPanelIn();
            AnimateContentPanelIn();
            StartCoroutine(ScrollToLevel());
            PlayLevelSelectorUnlockAnimationIfAvailable(_unlockAnimationIndex);
        }

        public void SwitchToGameplayScreen()
        {
            AnimateLevelSelectionPanelOut();
            StartCoroutine(AnimateGameplayScreenIn());
            ToggleScoreText(true);
            SetTitleText();
        }
        
        public void SwitchToLevelEndScreen()
        {
            StartCoroutine(AnimateGameplayScreenOut());
            StartCoroutine(AnimateAndActivateLevelEndScreenIn());
            GetLastScore();
        }
        
        private void SwitchToLevelSelectionScreen()
        { 
            ResetLevelElements(); 
            AnimateAndDeactivateLevelEndScreenOut();
            UpdateHighScoresInLevelSelectors();
            StartCoroutine(AnimateIntroScreenIn());
        }

        private void ResetLevelElements()
        {
            _sentWordsDisplay.ResetSentWords();
        }

        #region Scores

        public void UpdateScoreText(int score)
        {
            scoreText.text = $"Score: {score}";
            wordScoreText.text = "";
        }
        public void UpdateScoreText(int score, int wordScore)
        {
            scoreText.text = $"Score: {score} ";
            wordScoreText.text = $"Word Score: {wordScore}"; 
        }
        
        public void ToggleScoreText(bool state)
        {
            scoreText.gameObject.SetActive(state);
        }

        #endregion

        #region LevelSelectionScreen

        private IEnumerator AnimateIntroScreenIn()
        {
            yield return new WaitForSeconds(1); // Wait for the end screen to animate out
            levelsButton.gameObject.SetActive(true);
        }
          private void PopulateLevelSelectionPanel()
        {
            for (var index = 0; index < _levelDatas.levelDatas.Length; index++)
            {
                var puzzleData = _levelDatas.GetLevelData(index);
                var levelSelector = Instantiate(levelSelectorPrefab, contentPanel);
                var isLevelLocked = _saveManager.IsLevelLocked(index);
                levelSelector.InitializeLevelSelector(puzzleData.title, index, isLevelLocked, _levelManager, _saveManager);
                _levelSelectors.Add(levelSelector);
            }
        }

        private void UpdateHighScoresInLevelSelectors()
        {
            foreach (var levelSelector in _levelSelectors)
            {
                levelSelector.UpdateHighScores();
            }
        }
        private void SetPopupPanel(bool state)
        {
            levelSelectionPanel.SetActive(state);
        }
        
        private IEnumerator ScrollToLevel()
        {
            yield return new WaitForSeconds(.75f); // Wait for the level selectors to populate
            var activeLevelIndex = _levelManager.GetCurrentLevelIndex();
            scrollRect.DOVerticalNormalizedPos(1 - (activeLevelIndex / 20f), .5f).SetEase(Ease.InOutCirc);
        }
    
        private void PlayLevelSelectorUnlockAnimationIfAvailable(int levelIndex)
        {
            if(_unlockAnimationIndex == -1) return; 
            _levelSelectors[levelIndex].PlayUnlockLevelAnimations();
            _unlockAnimationIndex = -1;
        }

        private void AnimateContentPanelIn()
        {
            for (var i = 0; i < contentPanel.transform.childCount; i++)
            {
                contentPanel.GetChild(i).transform.localPosition += Vector3.right * 5;
                contentPanel.GetChild(i).transform.DOLocalMoveX(0, .5f).SetEase(Ease.InOutCirc).SetDelay(i * .05f + .5f);
            }
        }

        private void AnimateLevelSelectionPanelIn()
        {
            for (var i = 0; i< levelSelectionPanel.transform.childCount; i++)
            {
                var pos = levelSelectionPanel.transform.GetChild(i).transform.position;
                levelSelectionPanel.transform.GetChild(i).transform.DOMoveY(pos.y - 150, .5f).SetEase(Ease.InOutCirc).SetDelay(i * .1f);
            }
        }

        private void AnimateLevelSelectionPanelOut()
        {
            for (var i = 0; i< levelSelectionPanel.transform.childCount; i++)
            {
                var pos = levelSelectionPanel.transform.GetChild(i).transform.position;
                levelSelectionPanel.transform.GetChild(i).transform.DOMoveY(pos.y + 150, .5f).SetEase(Ease.InOutCirc).SetDelay(i * .1f);
            }
        }

        #endregion

        #region GameplayScreen

        private IEnumerator AnimateGameplayScreenIn()
        {
            yield return new WaitForSeconds(1); // Wait for the level selection screen to animate out
            gameplayPanel.transform.DOLocalMoveX(0, .5f);
        }
        
        private IEnumerator AnimateGameplayScreenOut()
        {
            yield return new WaitForSeconds(1); // Wait for the gameplay screen to animate out
            gameplayPanel.transform.DOLocalMoveX(150, .5f);
        }
        
        private void SetTitleText()
        {
            var title = _levelDatas.GetLevelData(_levelManager.GetCurrentLevelIndex()).title;
            levelTitleText.text = title;
        }

        public void ToggleUndoButton(bool state)
        {
            undoButton.interactable = state;
        }
        #endregion

        #region LevelEndScreen

        private void UpdateLevelEndScoreTexts(int levelScore, int highScore)
        {
            levelScoreText.text = levelScore.ToString();
            highScoreText.text = highScore.ToString();

            if (levelScore <= highScore) return;
            highScoreText.text = levelScore.ToString();
            highScorePanel.SetActive(true);
            
            LogHighScoreEvent(levelScore);
        }

        private void LogHighScoreEvent(int levelScore)
        {
            _analyticsManager.LogEvent(_levelManager.GetCurrentLevelIndex(), LogEventTypes.HighScore, levelScore);
        }

        private IEnumerator AnimateAndActivateLevelEndScreenIn()
        {
            yield return new WaitForSeconds(2); // Wait for the gameplay screen to animate out
            

            levelEndPanel.SetActive(true);
            levelEndPanel.transform.DOLocalMoveX(0, .5f);
        }

        private void GetLastScore()
        {
            ToggleScoreText(false);
            var score = _scoreManager.GetLevelScore();
            var highScore = _saveManager.GetHighScore(_levelManager.GetCurrentLevelIndex());
            UpdateLevelEndScoreTexts(score, highScore);
            _scoreManager.CheckHighScore();
        }

        private void AnimateAndDeactivateLevelEndScreenOut()
        {
            levelEndPanel.transform.DOLocalMoveX(-150, .5f).OnComplete(() =>
            {
                levelEndPanel.SetActive(false);
            });
        }

        #endregion
        

    }
}
