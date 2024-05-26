using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame
{
    public class LevelSelector : MonoBehaviour
    {
        [Header("UI Elements")] [SerializeField]
        private TextMeshProUGUI levelIndexText;

        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button levelPlayButton;
        [SerializeField] private GameObject PlayButtonText;
        [SerializeField] private Image LockImage;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Color unlockedColor, lockedColor;

        private int _levelId;
        private LevelManager _levelManager;
        private SaveManager _saveManager;

        public void InitializeLevelSelector(string levelTitle, int levelId, bool locked, LevelManager levelManager,
            SaveManager saveManager)
        {
            _levelId = levelId;

            AssignScripts(levelManager, saveManager);
            DisplayIfHighScoreExists();
            InitializeUI(levelTitle, levelId, locked);
        }

        private void AssignScripts(LevelManager levelManager, SaveManager saveManager)
        {
            _levelManager = levelManager;
            _saveManager = saveManager;
        }

        public void UpdateHighScores()
        {
            DisplayIfHighScoreExists();
        }

        private void DisplayIfHighScoreExists()
        {
            var highScore = _saveManager.GetHighScore(_levelId);
            highScoreText.text = highScore == 0 ? "" : "High score: " + highScore;
        }

        private void InitializeUI(string levelTitle, int levelId, bool locked)
        {
            SetLevelIndexText(levelId);
            SetTitle(levelTitle);
            SetPlayButtonAndLock(locked);
            buttonImage.color = locked ? lockedColor : unlockedColor;
        }

        private void SetPlayButtonAndLock(bool locked)
        {
            PlayButtonText.SetActive(!locked);
            LockImage.gameObject.SetActive(locked);
            if (!locked) levelPlayButton.onClick.AddListener(ButtonAction);
        }

        private void SetLevelIndexText(int levelId)
        {
            levelIndexText.text = (levelId + 1).ToString();
        }

        private void SetTitle(string levelTitle)
        {
            levelText.text = levelTitle;
        }

        void ButtonAction()
        {
            _levelManager.LoadLevel(_levelId);
        }

        public void PlayUnlockLevelAnimations()
        {
            buttonImage.color = lockedColor;
            LockImage.gameObject.SetActive(true);
            PlayButtonText.SetActive(false);
            levelPlayButton.onClick.RemoveAllListeners();
            Invoke(nameof(UnlockAnimations), 1);
        }

        private void UnlockAnimations()
        {
            LockImage.DOFade(0, .5f).OnComplete(() => { PlayButtonText.SetActive(true); });
            buttonImage.DOColor(unlockedColor, .5f);

            levelPlayButton.onClick.AddListener(ButtonAction);
        }
    }
}
