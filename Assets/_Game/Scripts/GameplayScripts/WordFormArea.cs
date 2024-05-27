using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame
{
    public class WordFormArea : MonoBehaviour, IInitializable
    {
        [Header("Managers & Components")] private TileManager _tileManager;
        private SentWordsDisplay _sentWordsDisplay;
        private ScoreManager _scoreManager;
        private UIManager _uiManager;
        private TilePooler _tilePooler;
        private WordService _wordService;
        
        private readonly List<LetterTile> _letterTiles = new List<LetterTile>();
        private readonly Stack<MoveRecord> _moveHistory = new Stack<MoveRecord>();

        [Header("UI Elements")] 
        [SerializeField] private Image validityCheckerImage;

        [SerializeField] private Sprite validWordSprite;
        [SerializeField] private Sprite invalidWordSprite;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button sendButton;

        [Header("Gameplay Elements")] [SerializeField]
        private Image[] tileSlots;

        [SerializeField] private GameObject sentWordPrefab;
        [SerializeField] private TextAsset dictionary;

        [Header("Settings")] 
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Color lightColor = new Color(0, 1, 0, .7f);
        private bool _areaAvailable = true;

        public void Initialize(GameScriptsConfig config)
        {
            AssignScripts(config);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _wordService = _tileManager.GetWordService();
            AddButtonListeners();
        }

        private void AddButtonListeners()
        {
            undoButton.onClick.AddListener(UndoLastMove);
            sendButton.onClick.AddListener(SendWord);
        }

        private void AssignScripts(GameScriptsConfig config)
        {
            _tileManager = config.TileManager;
            _sentWordsDisplay = config.SentWordsDisplay;
            _scoreManager = config.ScoreManager;
            _tilePooler = config.TilePooler;
            _uiManager = config.UIManager;
        }

        #region Letter Management

        private void SendWord()
        {
            if (!_areaAvailable) return;
            _areaAvailable = false;

            var word = _letterTiles.Aggregate("", (current, letterTile) => current + letterTile.GetLetter().ToUpper());

            validityCheckerImage.sprite = invalidWordSprite;
            sendButton.interactable = false;
            ClearHistory();
            
            PlaySendAnimations(word);

            _sentWordsDisplay.AddWord(word);
            _tileManager.CheckLevelEnd();
            _scoreManager.ChangeScore();
        }

        private void ClearHistory()
        {
            _moveHistory.Clear();
            _uiManager.ToggleUndoButton(false);
        }

        public void TakeLetterTile(LetterTile letterTile, Transform originalParent, Transform originalTransform)
        {
            if (_letterTiles.Count > 6) return;
            _letterTiles.Add(letterTile);
            letterTile.SetInWordArea(true);
            var moveDuration = CalculateConstantSpeedDuration(letterTile, transform, moveSpeed);
            CreateHistoryPoint(letterTile, originalParent, originalTransform, moveDuration);

            MoveLetterTileToSlot(letterTile);
            CheckIfInDictionary();
        }

        private void CreateHistoryPoint(LetterTile letterTile, Transform originalParent, Transform originalTransform, float moveDuration)
        {
            _moveHistory.Push(new MoveRecord(letterTile, originalParent, originalTransform.position, moveDuration));
            _uiManager.ToggleUndoButton(_moveHistory.Count > 0);
        }

        #endregion

        #region Animations

        private void PlaySendAnimations(string word)
        {
            var sequence = DOTween.Sequence();

            for (var index = 0; index < _letterTiles.Count; index++)
            {
                var tile = _letterTiles[index];
                sequence.Join(tile.transform.DOLocalMove(Vector3.up * 3, .25f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetDelay(index * .1f)
                    .OnComplete(() => { _tilePooler.ReturnTile(tile); }));
            }

            sequence.AppendCallback(() =>
            {
                _letterTiles.Clear();
                AnimateLetterSlots();
            });

            sequence.Play();
            LightUpTiles(lightColor, word.Length);
        }

        private void LightUpTiles(Color color, int wordLength)
        {
            for (var index = 0; index < wordLength; index++)
            {
                var t = tileSlots[index];
                t.DOColor(color, .2f).SetDelay(index * .1f);
            }
        }

        private void AnimateLetterSlots()
        {
            for (var index = 0; index < tileSlots.Length; index++)
            {
                var t = tileSlots[index];
                t.transform.DORotate(t.transform.rotation.eulerAngles + new Vector3(0, 90, 0), .25f,
                        RotateMode.FastBeyond360)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(index * .1f)
                    .OnComplete(() => { _areaAvailable = true; });
            }

            Color ogColor = new Color(0, 0, 0, .7f);

            LightUpTiles(ogColor, 6);
        }

        private void MoveLetterTileToSlot(LetterTile letterTile)
        {
            var speed = moveSpeed;

            foreach (var t in tileSlots)
            {
                if (t.transform.childCount != 0) continue;

                letterTile.transform.SetParent(t.transform);
                t.transform.SetAsLastSibling();

                var duration = CalculateConstantSpeedDuration(letterTile, t.transform, speed);

                letterTile.transform.DOMove(t.transform.position, duration).SetEase(Ease.OutQuad);
                letterTile.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutQuad);

                break;
            }
        }

        #endregion

        #region Public Methods

        public bool IsAvailable()
        {
            return _areaAvailable;
        }

        public int GetCurrentLetterCount()
        {
            return _letterTiles.Count;
        }

        public bool HasLetters()
        {
            return _letterTiles.Count > 0;
        }

        #endregion

        #region Calculations

        private static float CalculateConstantSpeedDuration(LetterTile letterTile, Transform t, float speed)
        {
            var distance = Vector3.Distance(letterTile.transform.position, t.position);
            var duration = distance / speed;
            return duration;
        }

        private void CheckIfInDictionary()
        {
            if (_letterTiles.Count < 2)
            {
                validityCheckerImage.sprite = invalidWordSprite;
                sendButton.interactable = false;
                _scoreManager.CleanWordScore();
                return;
            }

            var word = _letterTiles.Aggregate("", (current, letterTile) => current + letterTile.GetLetter().ToUpper());
            var wordExists = _wordService.CheckIfInDictionary(word);

            validityCheckerImage.sprite = wordExists ? validWordSprite : invalidWordSprite;
            sendButton.interactable = wordExists;
            _scoreManager.ChangeWordScore(wordExists ? word : "");
        }

        #endregion

        #region Undo

        public void UndoLastMove()
        {
            if (_moveHistory.Count == 0)
            {
                _uiManager.ToggleUndoButton(false);
                return;
            }

            var lastMove = _moveHistory.Pop();
            var lastMovedTile = lastMove.LetterTile;
            var originalParent = lastMove.OriginalParent;
            var originalPosition = lastMove.OriginalPosition;
            var children = lastMove.Children;
            var duration = lastMove.MoveDuration;

            lastMovedTile.SetInWordArea(false);
            foreach (var child in children)
            {
                child.AddParent(lastMovedTile);
            }

            MoveTileBackToOriginalPosition(lastMovedTile, originalParent, originalPosition, duration);

            _letterTiles.Remove(lastMovedTile);
            CheckIfInDictionary();
            _uiManager.ToggleUndoButton(_moveHistory.Count > 0);
        }

        private static void MoveTileBackToOriginalPosition(LetterTile lastMovedTile, Transform originalParent,
            Vector3 originalPosition, float duration)
        {
            lastMovedTile.transform.SetParent(originalParent);
            lastMovedTile.MoveToOriginalPosition(originalPosition);
            lastMovedTile.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutQuad);
        }

        #endregion
    }
}
