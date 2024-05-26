using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileGame
{
    public class TileManager : MonoBehaviour, IInitializable
    {
        private WordService _wordService; 
        private PuzzleData _levelData;
    
        [Header("Managers & Components")]
        private ScoreManager _scoreManager;
        private WordFormArea _wordFormingArea;
        private LevelManager _levelManager;
        private TilePooler _tilePooler;
        
        [SerializeField] List<LetterTile> activeTiles = new List<LetterTile>();
    
        public WordService GetWordService() => _wordService ??= new WordService();
        
        #region Initialization
        
        public void Initialize(GameScriptsConfig config)
        {
            AssignManagers(config);
        }
        
        private void AssignManagers(GameScriptsConfig config)
        {
            _levelManager = config.LevelManager; 
            _scoreManager = config.ScoreManager;
            _wordFormingArea = config.WordFormArea;
            _tilePooler = config.TilePooler;
        }
        
        public void LoadLevel(PuzzleData levelData)
        {
            _levelData = levelData;
            GetWordService();
            InitializeTilesFromData();
        }

        private void InitializeTilesFromData()
        {
            var puzzleData = _levelData;
            var tileObjects = new Dictionary<int, LetterTile>();
        
            foreach (var tileData in puzzleData.tiles)
            {
                InitializeTile(tileData, tileObjects);
            }
        
            SortAndSetChildren(puzzleData, tileObjects);
        }
        

        private static void SortAndSetChildren(PuzzleData puzzleData, Dictionary<int, LetterTile> tileObjects)
        {
            foreach (var tileData in puzzleData.tiles)
            {
                tileObjects[tileData.id].SetChildren(tileData.children.Select(childId => tileObjects[childId]).ToList());
            }
            var sortedTiles = tileObjects.Values.OrderByDescending(tile => tile.transform.position.z).ToList();

            for (var i = 0; i < sortedTiles.Count; i++)
            {
                sortedTiles[i].transform.SetSiblingIndex(i);
            }
        }

        private void InitializeTile(TileData tileData, Dictionary<int, LetterTile> tileObjects)
        {
            var tileObject = _tilePooler.GetTile();
            tileObject.transform.position = tileData.position;
            tileObject.transform.SetParent(transform);
            tileObject.gameObject.SetActive(true);

            tileObject.name = "Tile_" + tileData.character;
            tileData.score = _scoreManager.GetScoreForLetter(tileData.character[0]);
            
            tileObject.InitializeTile(tileData, _wordFormingArea, this); 
            tileObjects[tileData.id] = tileObject;
            AddToActiveTiles(tileObject);
        }
    
        #endregion
    
        #region Tile Management

        public void MoveTile(LetterTile tile)
        {
            activeTiles.Remove(tile);
        }
    
        public void AddToActiveTiles(LetterTile letterTile)
        {
            activeTiles.Add(letterTile);
        }

        #endregion

        #region End Level Functions
        
        /// <summary>
        /// There is an issue with the code below
        /// Near finishing the level, if there is a possible word with the remaining tiles
        /// the level will not end. No matter if the letter the possible word starts with
        /// is locked or not.
        /// </summary>
        public void CheckLevelEnd()
        {
            if (activeTiles.Count <= 1)
            {
                EndLevel();
                return;
            }

            var allRelevantTiles = new List<LetterTile>();
            foreach (var tile in activeTiles)
            {
                AddTileAndChildren(tile, allRelevantTiles);
            }
        
            var letters = activeTiles
                // Should check if locked here
                .Select(tile => tile.GetLetter().ToUpper()[0])
                .ToList();

            if (!_wordService.CheckIfWordsCanBeFormed(letters))
            {
                EndLevel();
            }
        }
    
        // Recursively add tile and its children to the list
        private void AddTileAndChildren(LetterTile tile, List<LetterTile> allRelevantTiles)
        {
            if (!allRelevantTiles.Contains(tile))
            {
                allRelevantTiles.Add(tile);
            }

            foreach (var child in tile.GetChildren())
            {
                if (!allRelevantTiles.Contains(child))
                {
                    AddTileAndChildren(child, allRelevantTiles);
                }
            }
        }
        public int GetUnusedLetterCount()
        {
            return activeTiles.Count;
        }

        private void EndLevel()
        {
            ReturnAllTilesToPool();
            _levelManager.CompleteLevel();
        }
        
        private void ReturnAllTilesToPool()
        {
            foreach (var tile in activeTiles)
            {
                _tilePooler.ReturnTile(tile);
            }
            activeTiles.Clear();
        }
        
        #endregion



    }
}