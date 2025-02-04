using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models
{
    public record DailyGeneration(
        DateTime Date,
        double Energy,
        double Price
    );
}
