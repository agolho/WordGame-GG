using UnityEngine;

namespace TileGame
{
    public enum LogEventTypes
    {
        LevelStarted,
        LevelCompleted,
        HighScore
    }

    public class AnalyticsManager : MonoBehaviour
    {

        public void LogEvent(int activeLevelIndex, LogEventTypes logEventType, int score = 0)
        {
            if (logEventType == LogEventTypes.LevelStarted)
            {
                Debug.Log($"Level {activeLevelIndex} started");
            }
            if (logEventType == LogEventTypes.LevelCompleted)
            {
                Debug.Log($"Level {activeLevelIndex} completed");
            }
            if (logEventType == LogEventTypes.HighScore)
            {
                Debug.Log($"High score: {score}");
            }
        }
    }
}
