﻿using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Models;
using GeneratorDataProcessor.Models.GeneratorModels;
using GeneratorDataProcessor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Services
{
    public class FilePRocessingService : IFileProcesssing
    {
        private readonly ReferenceData _referenceData;
        public FilePRocessingService(ReferenceData referenceData)
        {
            _referenceData = referenceData;
        }
        public void ProcessGenerationReport(string inputFilePath, string OutPutFolder)
        {
            try
            {
                var generator = XMLHelper.ParseInputFile(inputFilePath);
                CalculateTotals(generator);

                var maxDailyEmissions = CalculateMaxDailyEmissions(generator);
                CalculateCoalHeatRates(generator);

                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string outputFileName = $"{fileName}-Results.xml";
                string outputPath = Path.Combine(OutPutFolder, outputFileName);
                XMLHelper.GenerateOutputFile(generator, maxDailyEmissions, outputPath, _referenceData);
            }
            catch (Exception ex)
            {
               throw new Exception("Error processing file", ex);
            }
        }
        private void CalculateTotals(List<GeneratorBase> generators)
        {
            Parallel.ForEach(generators, gen =>
            {
                var calculator = GeneratorCalculatorFactory.GetCalculator(gen);
                calculator?.CalculateTotalValue(gen, _referenceData);
            });
        }
        private Dictionary<DateTime, GeneratorBase> CalculateMaxDailyEmissions(List<GeneratorBase> generators)
        {
            var fossilGenerators = generators.Where(g => g.Type == GeneratorType.Gas || g.Type == GeneratorType.Coal);

            var dateEmissionMap = new Dictionary<DateTime, Dictionary<GeneratorBase, double>>();

            Parallel.ForEach(fossilGenerators, gen =>
            {
                var calculator = GeneratorCalculatorFactory.GetCalculator(gen);

                foreach (var day in gen.Generation)
                {
                    double dailyEmission = calculator.CalculateEmissions(gen, day, _referenceData);

                    if (!dateEmissionMap.ContainsKey(day.Date))
                    {
                        dateEmissionMap[day.Date] = new Dictionary<GeneratorBase, double>();
                    }

                    dateEmissionMap[day.Date][gen] = dailyEmission;
                }
            });

            var maxDailyEmissions = new Dictionary<DateTime, GeneratorBase>();
            foreach (var dateEntry in dateEmissionMap)
            {
                DateTime date = dateEntry.Key;
                var bestGen = dateEntry.Value.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
                maxDailyEmissions[date] = bestGen;
            }

            return maxDailyEmissions;
        }

        private void CalculateCoalHeatRates(List<GeneratorBase> generators)
        {
            var coalGens = generators.Where(g => g is CoalGenerator);
            Parallel.ForEach(coalGens, coalGen =>
            {
                var calculator = GeneratorCalculatorFactory.GetCalculator(coalGen);
                calculator?.CalculateHeatRate(coalGen);
            });
        }
    }
}
