using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Interfaces
{
    public interface IFileProcesssing
    {
        void ProcessGenerationReport(string inputFilePath,string OutPutFolder);
    }
}
