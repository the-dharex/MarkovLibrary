namespace MarkovLibrary.Models
{
    public class StateInfo
    {
        public string? State { get; set; }
        public int TransitionCount { get; set; }
        public int NextTokens { get; set; }
    }
}
