using UnityEngine;

namespace TileGame
{
    public class GameManager : MonoBehaviour, IInitializable
        {
            [Header("Managers")]
            private LevelManager _levelManager;
            private UIManager _uiManager;
            
            public void Initialize(GameScriptsConfig config)
            {
                AssignManagers(config);
            }

            private void AssignManagers(GameScriptsConfig config)
            {
                _levelManager = config.LevelManager;
                _uiManager = config.UIManager;
            }
            // TODO: game states
        }
}
