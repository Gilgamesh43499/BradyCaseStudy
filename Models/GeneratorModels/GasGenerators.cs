using GeneratorDataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models.GeneratorModels
{
    public class GasGenerator : GeneratorBase
    {
        public override GeneratorType Type => GeneratorType.Gas;
    }
}
