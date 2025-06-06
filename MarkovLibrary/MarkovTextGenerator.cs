using System.Text;
using System.Text.Json;
using MarkovLibrary.Models;

namespace MarkovLibrary
{
    /// <summary>
    /// Generador de texto usando cadenas de Markov
    /// </summary>
    public class MarkovTextGenerator
    {
        private readonly Dictionary<MarkovState, StateTransitions> _chain = new Dictionary<MarkovState, StateTransitions>();
        private readonly List<MarkovState> _startingStates = new List<MarkovState>();
        private readonly MarkovConfig _config;

        public int Order => _config.Order;
        public int StateCount => _chain.Count;
        public int StartingStateCount => _startingStates.Count;

        public MarkovTextGenerator(MarkovConfig? config = null)
        {
            _config = config ?? new MarkovConfig();
            if (_config.Order < 1)
                throw new ArgumentException("El orden debe ser mayor a 0", nameof(config));
        }

        /// <summary>
        /// Entrena la cadena con un texto
        /// </summary>
        public void Train(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("El texto no puede estar vacío", nameof(text));

            var tokens = TokenizeText(text);
            if (tokens.Count < _config.Order + 1)
                throw new MarkovException($"El texto debe tener al menos {_config.Order + 1} tokens para entrenar");

            TrainFromTokens(tokens);
        }

        /// <summary>
        /// Entrena la cadena con múltiples textos
        /// </summary>
        public void TrainBatch(IEnumerable<string> texts)
        {
            if (texts == null)
                throw new ArgumentNullException(nameof(texts));

            foreach (var text in texts.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                Train(text);
            }
        }

        /// <summary>
        /// Entrena desde un archivo
        /// </summary>
        public async Task TrainFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"No se encontró el archivo: {filePath}");

            var text = await File.ReadAllTextAsync(filePath);
            Train(text);
        }

        /// <summary>
        /// Genera texto usando la cadena entrenada
        /// </summary>
        public string GenerateText(int maxLength = 100, string? startWith = null)
        {
            if (_chain.Count == 0)
                throw new MarkovException("La cadena no ha sido entrenada");

            maxLength = Math.Min(maxLength, _config.MaxGenerationLength);
            var result = new List<string>();

            // Determinar estado inicial
            MarkovState currentState = GetInitialState(startWith);
            if (currentState == null)
                return string.Empty;

            // Agregar tokens del estado inicial
            result.AddRange(currentState.Tokens);

            // Generar texto
            while (result.Count < maxLength)
            {
                if (!_chain.TryGetValue(currentState, out StateTransitions? transitions))
                    break;

                string nextToken = transitions.SelectRandomTransition(_config.RandomGenerator, _config.MinProbabilityThreshold);
                if (nextToken == null)
                    break;

                result.Add(nextToken);

                // Crear nuevo estado
                var newStateTokens = currentState.Tokens.Skip(1).Concat(new[] { nextToken }).ToArray();
                currentState = new MarkovState(newStateTokens);

                // Verificar fin de oración
                if (_config.SentenceEnders.Contains(nextToken))
                {
                    // Posibilidad de terminar la generación
                    if (_config.RandomGenerator.NextDouble() < 0.3)
                        break;
                }
            }

            return JoinTokens(result);
        }

        /// <summary>
        /// Genera múltiples textos
        /// </summary>
        public IEnumerable<string> GenerateTexts(int count, int maxLength = 100, string? startWith = null)
        {
            for (int i = 0; i < count; i++)
            {
                yield return GenerateText(maxLength, startWith);
            }
        }

        /// <summary>
        /// Obtiene las probabilidades de los siguientes tokens para un estado dado
        /// </summary>
        public IEnumerable<(string Token, double Probability)> GetNextTokenProbabilities(string[] stateTokens)
        {
            if (stateTokens?.Length != _config.Order)
                throw new ArgumentException($"El estado debe tener exactamente {_config.Order} tokens");

            var state = new MarkovState(stateTokens);
            if (_chain.TryGetValue(state, out StateTransitions? transitions))
            {
                return transitions.GetProbabilities().OrderByDescending(p => p.Probability);
            }

            return Enumerable.Empty<(string, double)>();
        }

        /// <summary>
        /// Obtiene estadísticas de la cadena
        /// </summary>
        public MarkovStatistics GetStatistics()
        {
            return new MarkovStatistics
            {
                StateCount = _chain.Count,
                StartingStateCount = _startingStates.Count,
                TotalTransitions = _chain.Values.Sum(t => t.TotalCount),
                AverageTransitionsPerState = _chain.Count > 0 ? _chain.Values.Average(t => t.TotalCount) : 0,
                Order = _config.Order,
                MostCommonStates = _chain
                    .OrderByDescending(kvp => kvp.Value.TotalCount)
                    .Take(10)
                    .Select(kvp => new StateInfo
                    {
                        State = kvp.Key.ToString(),
                        TransitionCount = kvp.Value.TotalCount,
                        NextTokens = kvp.Value.Transitions.Count
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Guarda la cadena en un archivo JSON
        /// </summary>
        public async Task SaveToFileAsync(string filePath)
        {
            var data = new MarkovChainData
            {
                Order = _config.Order,
                States = _chain.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => kvp.Value.Transitions.ToDictionary(t => t.Key, t => t.Value)
                ),
                StartingStates = _startingStates.Select(s => s.ToString()).ToList()
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Carga la cadena desde un archivo JSON
        /// </summary>
        public async Task LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"No se encontró el archivo: {filePath}");

            var json = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<MarkovChainData>(json);

            if (data?.Order != _config.Order)
                throw new MarkovException($"El orden del archivo ({data?.Order ?? 0}) no coincide con la configuración ({_config.Order})");

            _chain.Clear();
            _startingStates.Clear();

            // Reconstruir cadena
            foreach (var stateData in data.States)
            {
                var stateTokens = stateData.Key.Split(' ');
                var state = new MarkovState(stateTokens);
                var transitions = new StateTransitions();

                foreach (var transition in stateData.Value)
                {
                    for (int i = 0; i < transition.Value; i++)
                    {
                        transitions.AddTransition(transition.Key);
                    }
                }

                _chain[state] = transitions;
            }

            // Reconstruir estados iniciales
            foreach (var startingState in data.StartingStates)
            {
                var tokens = startingState.Split(' ');
                _startingStates.Add(new MarkovState(tokens));
            }
        }

        /// <summary>
        /// Limpia la cadena entrenada
        /// </summary>
        public void Clear()
        {
            _chain.Clear();
            _startingStates.Clear();
        }

        private List<string> TokenizeText(string text)
        {
            if (!_config.CaseSensitive)
                text = text.ToLowerInvariant();

            // Tokenización básica por espacios, manteniendo puntuación
            var tokens = new List<string>();
            var currentToken = new StringBuilder();

            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    if (_config.PreserveWhitespace && c != ' ')
                    {
                        tokens.Add(c.ToString());
                    }
                }
                else if (char.IsPunctuation(c))
                {
                    if (currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    currentToken.Append(c);
                }
            }

            if (currentToken.Length > 0)
                tokens.Add(currentToken.ToString());

            return tokens.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }

        private void TrainFromTokens(List<string> tokens)
        {
            for (int i = 0; i <= tokens.Count - _config.Order - 1; i++)
            {
                var stateTokens = tokens.Skip(i).Take(_config.Order).ToArray();
                var nextToken = tokens[i + _config.Order];

                var state = new MarkovState(stateTokens);

                // Agregar a estados iniciales si es el comienzo
                if (i == 0)
                    _startingStates.Add(state);

                // Crear transición
                if (!_chain.TryGetValue(state, out StateTransitions? transitions))
                {
                    transitions = new StateTransitions();
                    _chain[state] = transitions;
                }

                transitions.AddTransition(nextToken);
            }
        }

        private MarkovState? GetInitialState(string? startWith)
        {
            if (!string.IsNullOrEmpty(startWith))
            {
                var startTokens = TokenizeText(startWith);
                if (startTokens.Count >= _config.Order)
                {
                    var stateTokens = startTokens.Take(_config.Order).ToArray();
                    var state = new MarkovState(stateTokens);
                    if (_chain.ContainsKey(state))
                        return state;
                }
            }

            // Usar estado inicial aleatorio
            return _startingStates.Count > 0
                ? _startingStates[_config.RandomGenerator.Next(_startingStates.Count)]
                : _chain.Keys.FirstOrDefault();
        }

        private string JoinTokens(List<string> tokens)
        {
            if (!tokens.Any()) return string.Empty;

            var result = new StringBuilder();
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                // No agregar espacio antes de puntuación
                if (i > 0 && !char.IsPunctuation(token[0]))
                {
                    result.Append(' ');
                }

                result.Append(token);
            }

            return result.ToString();
        }
    }
}
