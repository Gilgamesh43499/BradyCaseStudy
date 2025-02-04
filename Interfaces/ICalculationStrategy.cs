using GeneratorDataProcessor.Models;
using GeneratorDataProcessor.Models.GeneratorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Interfaces
{
    public interface ICalculationStrategy
    {
        void CalculateTotalValue(GeneratorBase generator, ReferenceData referenceData);
        double CalculateEmissions(GeneratorBase generator,DayData dayData, ReferenceData referenceData);
        void CalculateHeatRate(GeneratorBase generator);
    }
}
