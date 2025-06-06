namespace MarkovLibrary.Models
{
    /// <summary>
    /// Configuración para la cadena de Markov
    /// </summary>
    public class MarkovConfig
    {
        public int Order { get; set; } = 2;
        public bool CaseSensitive { get; set; } = false;
        public bool PreserveWhitespace { get; set; } = true;
        public string[] SentenceEnders { get; set; } = { ".", "!", "?" };
        public int MaxGenerationLength { get; set; } = 1000;
        public double MinProbabilityThreshold { get; set; } = 0.0;
        public Random RandomGenerator { get; set; } = new Random();
    }
}
