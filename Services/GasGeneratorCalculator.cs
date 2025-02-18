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
    public class GasGeneratorCalculator : ICalculationStrategy
    {
        public double CalculateEmissions(GeneratorBase generator, DayData dayData, ReferenceData referenceData)
        {
            var gas = generator as GasGenerator;
            if (gas == null || referenceData.EmissionFactor == null)
            {
                return 0;
            }
            double emissionFactor = referenceData.EmissionFactor.Medium;
            return dayData.Energy * emissionFactor * emissionFactor;
        }

        public void CalculateHeatRate(GeneratorBase generator)
        {

        }

        public void CalculateTotalValue(GeneratorBase generator, ReferenceData referenceData)
        {
            var gas = generator as GasGenerator;
            if (gas == null || referenceData.ValueFactor == null) return;

            double factor = referenceData.ValueFactor.Medium;

            double total = 0;
            foreach (var day in gas.Generation)
            {
                total += day.Energy * day.Price * factor;
            }

            gas.TotalGenerationValue = total;
        }
    }
}
