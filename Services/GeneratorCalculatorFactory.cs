using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Models;

namespace GeneratorDataProcessor.Services
{
    public static class GeneratorCalculatorFactory
    {
        public static ICalculationStrategy GetCalculator(IGenerator generator)
        {
            if (_calculators.TryGetValue(generator.Type, out var factory))
            {
                return factory();
            }
            throw new Exception("Generator type not supported");
        }

        private static readonly Dictionary<GeneratorType, Func<ICalculationStrategy>> _calculators =
            new Dictionary<GeneratorType, Func<ICalculationStrategy>> 
            {
                { GeneratorType.Wind, () => new WindGeneratorCalculator() },
                { GeneratorType.Gas, () => new GasGeneratorCalculator() },
                { GeneratorType.Coal, () => new CoalGeneratorCalculator() }
            };
    }
}
