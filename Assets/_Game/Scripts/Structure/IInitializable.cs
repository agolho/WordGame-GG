namespace TileGame
{
    /// <summary>
    /// Scripts that need to be initialized with a config object should implement this interface
    /// That way project avoids using Unity's Awake and Start methods
    /// </summary>
    public interface IInitializable
    {
        void Initialize(GameScriptsConfig config);
    }
}