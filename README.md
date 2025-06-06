# MarkovLibrary

Una biblioteca C# flexible y eficiente para generar texto utilizando Cadenas de Markov. Esta biblioteca proporciona una implementación robusta de generación de texto basada en Cadenas de Markov con parámetros configurables y APIs fáciles de usar.

## Características

- Orden configurable de las cadenas de Markov
- Opciones de sensibilidad a mayúsculas/minúsculas
- Preservación de espacios en blanco
- Finales de oración personalizables
- Soporte de serialización JSON para modelos entrenados
- Capacidades de entrenamiento por lotes
- Generación con texto inicial personalizado
- Métodos de extensión para uso rápido
- Análisis estadístico de las cadenas entrenadas

## Instalación

Agrega el proyecto MarkovLibrary a tu solución y referéncialo en tu proyecto.

## Inicio Rápido

```csharp
// Uso simple con métodos de extensión
string textoFuente = "Tu texto de entrenamiento aquí...";
string textoGenerado = textoFuente.GenerateMarkovText(length: 100, order: 2);
```

## Uso Básico

```csharp
// Crear un generador con configuración por defecto
var generador = new MarkovTextGenerator();

// Entrenar el generador
generador.Train("Tu texto de entrenamiento aquí...");

// Generar texto
string textoGenerado = generador.GenerateText(maxLength: 100);
```

## Configuración Avanzada

```csharp
var configuracion = new MarkovConfig
{
    Order = 2,                           // Número de tokens a considerar para predicciones
    CaseSensitive = false,              // Si se debe preservar mayúsculas/minúsculas
    PreserveWhitespace = true,          // Si se deben preservar caracteres especiales de espacio
    SentenceEnders = new[] { ".", "!", "?" }, // Caracteres que pueden finalizar una oración
    MaxGenerationLength = 1000,         // Longitud máxima del texto generado
    MinProbabilityThreshold = 0.0,      // Umbral mínimo de probabilidad para selección
    RandomGenerator = new Random()       // Generador de números aleatorios personalizado
};

var generador = new MarkovTextGenerator(configuracion);
```

## Métodos de Entrenamiento

```csharp
// Entrenar con un solo texto
generador.Train("Texto de entrenamiento...");

// Entrenar con múltiples textos
generador.TrainBatch(new[] { "Texto 1...", "Texto 2...", "Texto 3..." });

// Entrenar desde un archivo
await generador.TrainFromFileAsync("ruta/al/archivo/entrenamiento.txt");
```

## Generación de Texto

```csharp
// Generación básica
string texto = generador.GenerateText();

// Generación con longitud personalizada
string texto = generador.GenerateText(maxLength: 200);

// Generación con texto inicial
string texto = generador.GenerateText(maxLength: 200, startWith: "Había una vez");

// Generar múltiples textos
var textos = generador.GenerateTexts(count: 5, maxLength: 100).ToList();
```

## Persistencia del Modelo

```csharp
// Save trained model
await generator.SaveToFileAsync("model.json");

// Load trained model
await generator.LoadFromFileAsync("model.json");
```

## Statistics

```csharp
// Get chain statistics
var stats = generator.GetStatistics();
Console.WriteLine($"Total States: {stats.StateCount}");
Console.WriteLine($"Starting States: {stats.StartingStateCount}");
Console.WriteLine($"Total Transitions: {stats.TotalTransitions}");
Console.WriteLine($"Average Transitions Per State: {stats.AverageTransitionsPerState}");
```

## Advanced Analysis

```csharp
// Get next token probabilities for a specific state
var stateTokens = new[] { "the", "quick" };
var probabilities = generator.GetNextTokenProbabilities(stateTokens);
foreach (var (token, probability) in probabilities)
{
    Console.WriteLine($"Token: {token}, Probability: {probability}");
}
```

## Considerations

1. **Order Selection**: 
   - Higher orders (3+) produce more coherent but less creative text
   - Lower orders (1-2) produce more varied but potentially less coherent text
   - Order 2 is a good default for most use cases

2. **Training Data**:
   - Provide enough training data (at least order + 1 tokens)
   - More training data generally leads to better results
   - Quality of output depends heavily on input quality

3. **Performance**:
   - Memory usage increases with order and training data size
   - Generation time is generally fast and constant
   - Consider saving trained models for reuse

4. **Text Generation**:
   - Use `startWith` parameter to control generation context
   - Adjust `maxLength` based on your needs
   - Generated text will always end with a complete token

5. **Model Size**:
   - Models can be serialized to JSON for persistence
   - Larger training sets create larger models
   - Consider clearing the chain (`Clear()`) if memory is a concern

## Manejo de Errores

La biblioteca incluye un `MarkovException` personalizado para errores específicos. Excepciones comunes:

- Texto de entrenamiento vacío o nulo
- Tokens insuficientes para el entrenamiento
- Incompatibilidad de orden al cargar modelos
- Intentos de usar el generador sin entrenar

## Licencia

MIT License

Copyright (c) 2025

Se concede permiso, de forma gratuita, a cualquier persona que obtenga una copia de este software y archivos de documentación asociados (el "Software"), para utilizar el Software sin restricción, incluyendo sin limitación los derechos a usar, copiar, modificar, fusionar, publicar, distribuir, sublicenciar, y/o vender copias del Software, y permitir a las personas a las que se les proporcione el Software a hacer lo mismo, sujeto a las siguientes condiciones:

El aviso de copyright anterior y este aviso de permiso se incluirán en todas las copias o partes sustanciales del Software.

EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD, IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS AUTORES O TITULARES DEL COPYRIGHT SERÁN RESPONSABLES DE NINGUNA RECLAMACIÓN, DAÑOS U OTRAS RESPONSABILIDADES, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO O CUALQUIER OTRO MOTIVO, QUE SURJA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS TRATOS EN EL SOFTWARE.
