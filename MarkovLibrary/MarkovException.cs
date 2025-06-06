namespace MarkovLibrary
{
    /// <summary>
    /// Excepción personalizada para errores específicos de la cadena de Markov
    /// </summary>
    public class MarkovException : Exception
    {
        public MarkovException(string message) : base(message) { }
        public MarkovException(string message, Exception innerException) : base(message, innerException) { }
    }
}
