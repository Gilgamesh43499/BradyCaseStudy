using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Interfaces
{
    public interface IGenerationRepository
    {
        IEnumerable<IGenerator> LoadGenerators(string filePath);
    }
}
