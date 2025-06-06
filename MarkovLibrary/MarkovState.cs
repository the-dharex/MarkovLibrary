namespace MarkovLibrary
{
    /// <summary>
    /// Representa un estado en la cadena de Markov
    /// </summary>
    [Serializable]
    public class MarkovState : IEquatable<MarkovState>
    {
        public string[] Tokens { get; }
        public int Order => Tokens.Length;

        public MarkovState(string[] tokens)
        {
            Tokens = tokens?.ToArray() ?? throw new ArgumentNullException(nameof(tokens));
        }

        public MarkovState(IEnumerable<string> tokens) : this(tokens?.ToArray()) { }

        public override bool Equals(object obj) => Equals(obj as MarkovState);

        public bool Equals(MarkovState other)
        {
            if (other == null || Tokens.Length != other.Tokens.Length)
                return false;

            return Tokens.SequenceEqual(other.Tokens);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var token in Tokens)
                {
                    hash = hash * 31 + (token?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }

        public override string ToString() => string.Join(" ", Tokens);
    }
}
