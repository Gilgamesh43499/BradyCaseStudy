using GeneratorDataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models.GeneratorModels
{
    public class CoalGenerator : GeneratorBase
    {
        public double TotalHeatInput { get; set; }
        public double ActualNetGeneration { get; set; }
        public double ActualHeatRate { get; set; }

        public override GeneratorType Type => GeneratorType.Coal;
    }
}
