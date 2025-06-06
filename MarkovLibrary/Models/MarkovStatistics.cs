namespace MarkovLibrary.Models
{
    /// <summary>
    /// Estadísticas de la cadena de Markov
    /// </summary>
    public class MarkovStatistics
    {
        public int StateCount { get; set; }
        public int StartingStateCount { get; set; }
        public int TotalTransitions { get; set; }
        public double AverageTransitionsPerState { get; set; }
        public int Order { get; set; }
        public List<StateInfo> MostCommonStates { get; set; } = new List<StateInfo>();
    }
}
