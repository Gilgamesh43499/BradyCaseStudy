using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Models;
using GeneratorDataProcessor.Models.GeneratorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Services
{
    public class WindGeneratorCalculator : ICalculationStrategy
    {
        public double CalculateEmissions(GeneratorBase generator, DayData dayData, ReferenceData referenceData)
        {
            return 0;
        }

        public void CalculateHeatRate(GeneratorBase generator)
        {
            
        }

        public void CalculateTotalValue(GeneratorBase generator, ReferenceData referenceData)
        {
            var wind = generator as WindGenerator;
            if (wind == null)
            {
                throw new ArgumentException("Invalid generator type");
            }
            if (wind.Location == null)
            {
                throw new ArgumentException("Location cannot be null");
            }
            if (referenceData.ValueFactor == null)
            {
                throw new ArgumentException("ValueFactor cannot be null");
            }
            double factor = (wind.Location.ToLower() == ("offshore"))
                ? referenceData.ValueFactor.Low : referenceData.ValueFactor.High;

            double total = 0;
            foreach (var day in wind.Generation)
            {
                total += day.Energy * day.Price * factor;
            }

            wind.TotalGenerationValue = total;
        }
    }
}
