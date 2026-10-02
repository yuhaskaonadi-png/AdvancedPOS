using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace AdvancedPOS.Helpers
{
    public class SalesForecastResult
    {
        public List<float> ForecastedSales { get; set; }
        public List<float> LowerBound { get; set; }
        public List<float> UpperBound { get; set; }
    }

    public static class SalesForecastHelper
    {
        private class SalesData
        {
            public float Sales { get; set; }
        }

        private class SalesPrediction
        {
            public float[] ForecastedSales { get; set; }
            public float[] LowerBoundSales { get; set; }
            public float[] UpperBoundSales { get; set; }
        }

        // ==========================================
        // Historical Sales Data (History) බලලා, ඉදිරි 'horizon' දවස් ගණන Predict කිරීම
        // ==========================================
        public static SalesForecastResult ForecastSales(List<float> history, int horizon)
        {
            if (history == null || history.Count < 14)
            {
                throw new Exception("Not enough historical sales data to generate a forecast. At least 14 days of data is required.");
            }

            var mlContext = new MLContext(seed: 0);

            var data = history.Select(v => new SalesData { Sales = v }).ToList();
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            int seriesLength = history.Count;
            int windowSize = Math.Max(2, Math.Min(7, seriesLength / 3));

            var pipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(SalesPrediction.ForecastedSales),
                inputColumnName: nameof(SalesData.Sales),
                windowSize: windowSize,
                seriesLength: seriesLength,
                trainSize: seriesLength,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(SalesPrediction.LowerBoundSales),
                confidenceUpperBoundColumn: nameof(SalesPrediction.UpperBoundSales));

            var model = pipeline.Fit(dataView);
            var forecastEngine = model.CreateTimeSeriesEngine<SalesData, SalesPrediction>(mlContext);
            var forecast = forecastEngine.Predict();

            // Negative Values (Sales කියන්නේ Negative වෙන්න බෑ) 0ට හරවනවා
            var cleanedForecast = forecast.ForecastedSales.Select(v => Math.Max(0, v)).ToList();
            var cleanedLower = forecast.LowerBoundSales.Select(v => Math.Max(0, v)).ToList();
            var cleanedUpper = forecast.UpperBoundSales.Select(v => Math.Max(0, v)).ToList();

            return new SalesForecastResult
            {
                ForecastedSales = cleanedForecast,
                LowerBound = cleanedLower,
                UpperBound = cleanedUpper
            };
        }
    }
}
