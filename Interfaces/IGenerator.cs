using GeneratorDataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Interfaces
{
    public interface IGenerator
    {
        string Name { get; set; }
        GeneratorType Type { get; }
        //IEnumerable<DailyGeneration> Generations { get; set; }
        double EmissionsRating { get; set; }
    }
}
