using GeneratorDataProcessor.Models;
using GeneratorDataProcessor.Models.GeneratorModels;
using GeneratorDataProcessor.Services;
using System.Xml.Linq;

namespace GeneratorDataProcessor.Utilities
{
    public static class XMLHelper
    {
        public static List<GeneratorBase> ParseInputFile(string path)
        {
            var doc = XDocument.Load(path);
            var generators = new List<GeneratorBase>();

            foreach (var element in doc.Descendants("WindGenerator"))
            {
                var wind = new WindGenerator
                {
                    Name = element.Element("Name")?.Value ?? string.Empty,
                    Location = element.Element("Location")?.Value ?? string.Empty,
                    Generation = ParseDays(element.Element("Generation"))
                };
                generators.Add(wind);
            }

            foreach (var el in doc.Descendants("GasGenerator"))
            {
                var gas = new GasGenerator
                {
                    Name = el.Element("Name")?.Value ?? string.Empty,
                    EmissionsRating = (double?)el.Element("EmissionsRating") ?? 0.0,
                    Generation = ParseDays(el.Element("Generation"))
                };
                generators.Add(gas);
            }

            foreach (var el in doc.Descendants("CoalGenerator"))
            {
                var coal = new CoalGenerator
                {
                    Name = el.Element("Name")?.Value ?? string.Empty,
                    EmissionsRating = (double?)el.Element("EmissionsRating") ?? 0.0,
                    TotalHeatInput = (double?)el.Element("TotalHeatInput") ?? 0.0,
                    ActualNetGeneration = (double?)el.Element("ActualNetGeneration") ?? 0.0,
                    Generation = ParseDays(el.Element("Generation"))
                };
                generators.Add(coal);
            }
            return generators;
        }
        private static List<DayData> ParseDays(XElement? generationEl)
        {
            var days = new List<DayData>();
            if (generationEl == null) return days;

            foreach (var dayEl in generationEl.Descendants("Day"))
            {
                days.Add(new DayData
                {
                    Date = (DateTime?)dayEl.Element("Date") ?? DateTime.MinValue,
                    Energy = (double?)dayEl.Element("Energy") ?? 0.0,
                    Price = (double?)dayEl.Element("Price") ?? 0.0
                });
            }
            return days;
        }
        public static ReferenceData ParseReferenceData(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var valFactor = doc.Descendants("ValueFactor").FirstOrDefault();
            var emiFactor = doc.Descendants("EmissionsFactor").FirstOrDefault();

            if (valFactor == null || emiFactor == null)
            {
                throw new InvalidOperationException("Missing required elements in the XML file.");
            }

            var referenceData = new ReferenceData
            {
                ValueFactor = new Factor
                {
                    High = (double?)valFactor.Element("High") ?? 0.0,
                    Medium = (double?)valFactor.Element("Medium") ?? 0.0,
                    Low = (double?)valFactor.Element("Low") ?? 0.0
                },
                EmissionFactor = new Factor
                {
                    High = (double?)emiFactor.Element("High") ?? 0.0,
                    Medium = (double?)emiFactor.Element("Medium") ?? 0.0,
                    Low = (double?)emiFactor.Element("Low") ?? 0.0
                }
            };

            return referenceData;
        }
        public static void GenerateOutputFile(List<GeneratorBase> generators,
                                              Dictionary<DateTime, GeneratorBase> maxDailyEmissions,
                                              string outputFilePath)
        {
            var root = new XElement("GenerationOutput",
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema")
            );

            var totalsEl = new XElement("Totals");
            foreach (var gen in generators)
            {
                var genEl = new XElement("Generator",
                    new XElement("Name", gen.Name),
                    new XElement("Total", gen.TotalGenerationValue.ToString("F12"))
                );
                totalsEl.Add(genEl);
            }
            root.Add(totalsEl);

            var maxEmissionEl = new XElement("MaxEmissionGenerators");
            var allDates = maxDailyEmissions.Keys.OrderBy(d => d).ToList();
            foreach (var date in allDates)
            {
                var gen = maxDailyEmissions[date];
                double emissionVal = 0;
                var dayData = gen.Generation.FirstOrDefault(d => d.Date == date);
                if (dayData != null)
                {
                    var calc = GeneratorCalculatorFactory.GetCalculator(gen);
                    emissionVal = calc.CalculateEmissions(gen, dayData, new()); 
                }

                var dayEl = new XElement("Day",
                    new XElement("Name", gen.Name),
                    new XElement("Date", date.ToString("yyyy-MM-ddTHH:mm:ssK")),
                    new XElement("Emission", emissionVal.ToString("F12"))
                );
                maxEmissionEl.Add(dayEl);
            }
            root.Add(maxEmissionEl);

            var heatRatesEl = new XElement("ActualHeatRates");
            var coalGenerators = generators.OfType<CoalGenerator>().ToList();
            foreach (var coal in coalGenerators)
            {
                var hrEl = new XElement("ActualHeatRate",
                    new XElement("Name", coal.Name),
                    new XElement("HeatRate", coal.ActualHeatRate.ToString("F12"))
                );
                heatRatesEl.Add(hrEl);
            }
            root.Add(heatRatesEl);

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            doc.Save(outputFilePath);
        }
    }
}
