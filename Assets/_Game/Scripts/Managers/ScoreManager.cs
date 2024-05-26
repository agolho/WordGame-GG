using System.Linq;
using UnityEngine;

namespace TileGame
{
    public class ScoreManager : MonoBehaviour, IInitializable
    {
        private UIManager _uiManager;
        private TileManager _tileManager;
        
        private readonly LetterScoreData _letterScoreData = new LetterScoreData();
        private int _score;
        private int _wordScore;
        
        #region Public Methods

        public void Initialize(GameScriptsConfig config)
        {
            _score = 0;
            _uiManager = config.UIManager;
            _tileManager = config.TileManager;
            
            UpdateScoreText();
        }
        
        public void ChangeScore()
        {
            _score += _wordScore;
            _wordScore = 0;
            UpdateScoreText();
        }
    
        public void ChangeWordScore(string word)
        {
            if(word == "") {
                _wordScore = 0;
                UpdateScoreText();
                return;
            }
        
            var score = CalculateWordScore(word);
            _wordScore = score;
            UpdateScoreText();
        }
    
        private int CalculateWordScore(string word)
        {
            var wordScore = word.Sum(GetScoreForLetter);
            wordScore *= word.Length * 10;
            return wordScore;
        }
    
        public int GetScoreForLetter(char letter)
        {
            return _letterScoreData.GetScoreForLetter(letter);
        }
    
        public int CalculateLevelScore()
        {
            var penalty = CalculatePenalty();
            
            _score += _wordScore;
            _score -= penalty;
            if (_score < 0) _score = 0;
    
            _uiManager.ToggleScoreText(false);
            return _score;
        }

        private int CalculatePenalty()
        {
            var unusedLetterCount = _tileManager.GetUnusedLetterCount();
            return 100 * unusedLetterCount;
        }
    
        public void CleanWordScore()
        {
            _wordScore = 0;
            UpdateScoreText();
        }
        
        public void ResetScore()
        {
            _score = 0;
            _wordScore = 0;
            UpdateScoreText();
        }

        #endregion
        
        private void UpdateScoreText()
        {
            if (_wordScore == 0)
            {
                _uiManager.UpdateScoreText(_score);
            }
            else
            {
                _uiManager.UpdateScoreText(_score, _wordScore);
            }
        }

        public int GetLevelScore()
        {
            return _score;
        }
    }
}