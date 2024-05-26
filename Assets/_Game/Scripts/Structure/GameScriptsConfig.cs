namespace TileGame
{
    /// <summary>
    /// All the game scripts that need to be passed around
    /// Expandable if more scripts are added
    /// </summary>
    public class GameScriptsConfig
    {
        public LevelManager LevelManager { get; set; }
        public GameManager GameManager { get; set; }
        public UIManager UIManager { get; set; }
        public TileManager TileManager { get; set; }
        public LevelDatas LevelDatas { get; set; }
        public WordFormArea WordFormArea { get; set; }
        public SentWordsDisplay SentWordsDisplay { get; set; }
        public ScoreManager ScoreManager { get; set; }
        public SaveManager SaveManager { get; set; }
        public TilePooler TilePooler { get; set; }
        public AnalyticsManager AnalyticsManager { get; set; }
        
        public GameScriptsConfig(
            LevelManager levelManager, 
            GameManager gameManager, 
            UIManager uiManager, 
            TileManager tileManager, 
            LevelDatas levelDatas,
            WordFormArea wordFormArea,
            SentWordsDisplay sentWordsDisplay,
            ScoreManager scoreManager,
            SaveManager saveManager,
            TilePooler tilePooler,
            AnalyticsManager analyticsManager
            )
        {
            LevelManager = levelManager;
            GameManager = gameManager;
            UIManager = uiManager;
            TileManager = tileManager;
            LevelDatas = levelDatas;
            WordFormArea = wordFormArea;
            SentWordsDisplay = sentWordsDisplay;
            ScoreManager = scoreManager;
            SaveManager = saveManager;
            TilePooler = tilePooler;
            AnalyticsManager = analyticsManager;
        }
    }
}