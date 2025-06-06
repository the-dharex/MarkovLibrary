namespace MarkovLibrary.Models
{
    /// <summary>
    /// Datos para serialización
    /// </summary>
    [Serializable]
    public class MarkovChainData
    {
        public int Order { get; set; }
        public Dictionary<string, Dictionary<string, int>> States { get; set; } = new Dictionary<string, Dictionary<string, int>>();
        public List<string> StartingStates { get; set; } = new List<string>();
    }
}
