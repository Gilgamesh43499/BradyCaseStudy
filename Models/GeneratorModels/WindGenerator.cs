using GeneratorDataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models.GeneratorModels
{
    public class WindGenerator : GeneratorBase
    {
        public string? Location { get; set; } // "Onshore" or "Offshore"

        public override GeneratorType Type => GeneratorType.Wind;
    }
}
