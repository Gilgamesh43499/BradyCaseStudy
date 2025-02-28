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
    public class CoalGeneratorCalculator : ICalculationStrategy
    {
        public double CalculateEmissions(GeneratorBase generator, DayData dayData, ReferenceData referenceData)
        {
            var coal = generator as CoalGenerator;
            if (coal == null || referenceData.EmissionFactor == null)
            {
                return 0;
            }
            double emissionFactor = referenceData.EmissionFactor.High;
            return dayData.Energy * emissionFactor * generator.EmissionsRating;
        }

        public void CalculateHeatRate(GeneratorBase generator)
        {
            var coal = generator as CoalGenerator;
            if (coal == null)
            {
                throw new ArgumentException("Invalid generator type");
            }
            if (coal.ActualNetGeneration != 0)
            {
                coal.ActualHeatRate = coal.TotalHeatInput / coal.ActualNetGeneration;
            }
            else
            {
                coal.ActualHeatRate = 0;
            }
        }

        public void CalculateTotalValue(GeneratorBase generator, ReferenceData referenceData)
        {
            var coal = generator as CoalGenerator;
            if (coal == null || referenceData.ValueFactor == null)
            {
                throw new ArgumentException("Invalid generator type");
            }
            double factor = referenceData.ValueFactor.Medium;
            double total = 0;
            foreach (var day in coal.Generation)
            {
                total += day.Energy * day.Price * factor;
            }

            coal.TotalGenerationValue = total;
        }
    }
}
