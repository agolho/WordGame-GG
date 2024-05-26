using UnityEngine;

namespace TileGame
{
    public class Initializer : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private TileManager tileManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private AnalyticsManager analyticsManager;
        
        [Header("Gameplay Scripts")]
        [SerializeField] private WordFormArea wordFormArea;
        [SerializeField] private SentWordsDisplay sentWordsDisplay;
        [SerializeField] private TilePooler tilePooler;
        
        [Header("Data")]
        [SerializeField] private LevelDatas levelDatas;
        
        
        /// <summary>
        /// Entry point of the game
        /// </summary>
        private void Start()
        {
            InitializeComponents();
        }

        void InitializeComponents()
        {
            var config = new GameScriptsConfig(
                levelManager, 
                gameManager, 
                uiManager, 
                tileManager, 
                levelDatas,
                wordFormArea,
                sentWordsDisplay,
                scoreManager,
                saveManager,
                tilePooler,
                analyticsManager
                );
            
            InitializeScript(gameManager, config);
            InitializeScript(uiManager, config);
            InitializeScript(levelManager, config);
            InitializeScript(tileManager, config);
            InitializeScript(scoreManager, config);
            InitializeScript(wordFormArea, config);
            InitializeScript(saveManager, config);
            InitializeScript(tileManager, config);
        }

        private static void InitializeScript(IInitializable initializableScript, GameScriptsConfig config)
        {
            initializableScript?.Initialize(config);
        }
    }
}