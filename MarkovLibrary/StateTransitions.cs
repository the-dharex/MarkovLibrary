namespace MarkovLibrary
{
    /// <summary>
    /// Representa las transiciones posibles desde un estado
    /// </summary>
    [Serializable]
    public class StateTransitions
    {
        private readonly Dictionary<string, int> _transitions = new Dictionary<string, int>();
        private int _totalCount = 0;

        public IReadOnlyDictionary<string, int> Transitions => _transitions;
        public int TotalCount => _totalCount;

        public void AddTransition(string nextToken)
        {
            if (nextToken == null) return;

            if (_transitions.ContainsKey(nextToken))
                _transitions[nextToken]++;
            else
                _transitions[nextToken] = 1;

            _totalCount++;
        }

        public double GetProbability(string nextToken)
        {
            if (_totalCount == 0) return 0.0;
            return _transitions.TryGetValue(nextToken, out int count) ? (double)count / _totalCount : 0.0;
        }

        public string? SelectRandomTransition(Random random, double minProbability = 0.0)
        {
            if (_totalCount == 0) return null;

            var validTransitions = _transitions.Where(kvp => GetProbability(kvp.Key) >= minProbability).ToList();
            if (!validTransitions.Any()) return null;

            int randomValue = random.Next(validTransitions.Sum(kvp => kvp.Value));
            int currentSum = 0;

            foreach (var kvp in validTransitions)
            {
                currentSum += kvp.Value;
                if (randomValue < currentSum)
                    return kvp.Key;
            }

            return validTransitions.Last().Key;
        }

        public IEnumerable<(string Token, double Probability)> GetProbabilities()
        {
            return _transitions.Select(kvp => (kvp.Key, GetProbability(kvp.Key)));
        }
    }
}
