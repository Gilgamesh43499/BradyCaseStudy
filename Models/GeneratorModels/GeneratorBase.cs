using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models.GeneratorModels
{
    public abstract class GeneratorBase : IGenerator
    {
        public required string Name { get; set; }
        public double EmissionsRating { get; set; }

        public List<DayData> Generation { get; set; } = new List<DayData>();

        public double TotalGenerationValue { get; set; }

        public abstract GeneratorType Type { get; }
    }
}
