using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models
{
    public enum GeneratorType
    {
        Wind,
        Gas,
        Coal
    }
    public enum ValueFactorLevel
    {
        Low,
        Medium,
        High
    }
    public enum EmissionsFactorLevel
    {
        Low,
        Medium,
        High
    }
}