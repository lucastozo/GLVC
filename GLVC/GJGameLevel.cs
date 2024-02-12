namespace GLVC
{
    public class GjGameLevel
    {
        public string? LevelString { get; set; }
        public int AudioTrack { get; set; }
        public List<string> Objects { get; set; } = new();
    }
}
