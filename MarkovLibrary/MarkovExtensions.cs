using MarkovLibrary.Models;

namespace MarkovLibrary
{
    /// <summary>
    /// Clase de extensión para facilitar el uso
    /// </summary>
    public static class MarkovExtensions
    {
        /// <summary>
        /// Crea un generador de Markov con configuración por defecto
        /// </summary>
        public static MarkovTextGenerator CreateMarkovGenerator(this string text, int order = 2)
        {
            var generator = new MarkovTextGenerator(new MarkovConfig { Order = order });
            generator.Train(text);
            return generator;
        }

        /// <summary>
        /// Genera texto directamente desde un string
        /// </summary>
        public static string GenerateMarkovText(this string text, int length = 100, int order = 2)
        {
            return text.CreateMarkovGenerator(order).GenerateText(length);
        }
    }
}
