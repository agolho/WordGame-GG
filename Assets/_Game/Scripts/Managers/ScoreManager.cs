using System.Linq;
using UnityEngine;

namespace TileGame
{
    public class ScoreManager : MonoBehaviour, IInitializable
    {
        private UIManager _uiManager;
        private TileManager _tileManager;
        private SaveManager _saveManager;
        private LevelManager _levelManager;
        
        private readonly LetterScoreData _letterScoreData = new LetterScoreData();
        private int _score;
        private int _wordScore;
        
        #region Public Methods

        public void Initialize(GameScriptsConfig config)
        {
            _score = 0;
            AssignScripts(config);
            UpdateScoreText();
        }

        private void AssignScripts(GameScriptsConfig config)
        {
            _uiManager = config.UIManager;
            _tileManager = config.TileManager;
            _saveManager = config.SaveManager;
            _levelManager = config.LevelManager;
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

        private int CalculateLevelEndScore()
        {
            var penalty = CalculatePenalty();
      
            _score -= penalty;
            if (_score < 0) _score = 0;
    
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
            var finishingScore = CalculateLevelEndScore();
            return finishingScore;
        }

        public void CheckHighScore()
        {
            var levelIndex = _levelManager.GetCurrentLevelIndex();
            var highScore = _saveManager.GetHighScore(levelIndex);
            if(_score > highScore)
            {
                _saveManager.SetHighScore(levelIndex, _score);
            }
        }
    }
}