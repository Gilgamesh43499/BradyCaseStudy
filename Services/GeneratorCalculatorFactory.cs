using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Models;

namespace GeneratorDataProcessor.Services
{
    public static class GeneratorCalculatorFactory
    {
        public static ICalculationStrategy GetCalculator(IGenerator generator)
        {
            switch (generator.Type)
            {
                case GeneratorType.Wind:
                    return new WindGeneratorCalculator();
                case GeneratorType.Gas:
                    return new GasGeneratorCalculator();
                case GeneratorType.Coal:
                    return new CoalGeneratorCalculator();
                default:
                    throw new Exception("Invalid Generator");
            }
        }
    }
}
