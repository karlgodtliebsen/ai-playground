using System.Diagnostics;

using Microsoft.ML;
using Microsoft.ML.Data;

using Serilog;

namespace ML.Net.ImageClassification.Tests.Domain;

public static class ReportGenerator
{
    public static void PrintPrediction(string prediction)
    {
        Log.Logger.Information($"*************************************************");
        Log.Logger.Information($"Predicted : {prediction}");
        Log.Logger.Information($"*************************************************");
    }

    public static void PrintRegressionPredictionVersusObserved(string predictionCount, string observedCount)
    {
        Log.Logger.Information($"-------------------------------------------------");
        Log.Logger.Information($"Predicted : {predictionCount}");
        Log.Logger.Information($"Actual:     {observedCount}");
        Log.Logger.Information($"-------------------------------------------------");
    }

    public static void PrintRegressionMetrics(string name, RegressionMetrics metrics)
    {
        Log.Logger.Information($"*************************************************");
        Log.Logger.Information($"*       Metrics for {name} regression model      ");
        Log.Logger.Information($"*------------------------------------------------");
        Log.Logger.Information($"*       LossFn:        {metrics.LossFunction:0.##}");
        Log.Logger.Information($"*       R2 Score:      {metrics.RSquared:0.##}");
        Log.Logger.Information($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}");
        Log.Logger.Information($"*       Squared loss:  {metrics.MeanSquaredError:#.##}");
        Log.Logger.Information($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}");
        Log.Logger.Information($"*************************************************");
    }

    public static void PrintBinaryClassificationMetrics(string name, CalibratedBinaryClassificationMetrics metrics)
    {
        Log.Logger.Information($"************************************************************");
        Log.Logger.Information($"*       Metrics for {name} binary classification model      ");
        Log.Logger.Information($"*-----------------------------------------------------------");
        Log.Logger.Information($"*       Accuracy: {metrics.Accuracy:P2}");
        Log.Logger.Information($"*       Area Under Curve:      {metrics.AreaUnderRocCurve:P2}");
        Log.Logger.Information($"*       Area under Precision recall Curve:  {metrics.AreaUnderPrecisionRecallCurve:P2}");
        Log.Logger.Information($"*       F1Score:  {metrics.F1Score:P2}");
        Log.Logger.Information($"*       LogLoss:  {metrics.LogLoss:#.##}");
        Log.Logger.Information($"*       LogLossReduction:  {metrics.LogLossReduction:#.##}");
        Log.Logger.Information($"*       PositivePrecision:  {metrics.PositivePrecision:#.##}");
        Log.Logger.Information($"*       PositiveRecall:  {metrics.PositiveRecall:#.##}");
        Log.Logger.Information($"*       NegativePrecision:  {metrics.NegativePrecision:#.##}");
        Log.Logger.Information($"*       NegativeRecall:  {metrics.NegativeRecall:P2}");
        Log.Logger.Information($"************************************************************");
    }

    public static void PrintAnomalyDetectionMetrics(string name, AnomalyDetectionMetrics metrics)
    {
        Log.Logger.Information($"************************************************************");
        Log.Logger.Information($"*       Metrics for {name} anomaly detection model      ");
        Log.Logger.Information($"*-----------------------------------------------------------");
        Log.Logger.Information($"*       Area Under ROC Curve:                       {metrics.AreaUnderRocCurve:P2}");
        Log.Logger.Information($"*       Detection rate at false positive count: {metrics.DetectionRateAtFalsePositiveCount}");
        Log.Logger.Information($"************************************************************");
    }

    public static void PrintMultiClassClassificationMetrics(string name, MulticlassClassificationMetrics metrics)
    {
        Log.Logger.Information($"************************************************************");
        Log.Logger.Information($"*    Metrics for {name} multi-class classification model   ");
        Log.Logger.Information($"*-----------------------------------------------------------");
        Log.Logger.Information($"    AccuracyMacro = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
        Log.Logger.Information($"    AccuracyMicro = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
        Log.Logger.Information($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better");
        if (metrics.PerClassLogLoss.Count >= 1)
        {
            Log.Logger.Information($"    LogLoss for class 1 = {metrics.PerClassLogLoss[0]:0.####}, the closer to 0, the better");
        }

        if (metrics.PerClassLogLoss.Count >= 2)
        {
            Log.Logger.Information($"    LogLoss for class 2 = {metrics.PerClassLogLoss[1]:0.####}, the closer to 0, the better");
        }

        if (metrics.PerClassLogLoss.Count >= 3)
        {
            Log.Logger.Information($"    LogLoss for class 3 = {metrics.PerClassLogLoss[2]:0.####}, the closer to 0, the better");
        }
        Log.Logger.Information($"************************************************************");
    }

    public static void PrintRegressionFoldsAverageMetrics(string algorithmName, IReadOnlyList<TrainCatalogBase.CrossValidationResult<RegressionMetrics>> crossValidationResults)
    {
        var L1 = crossValidationResults.Select(r => r.Metrics.MeanAbsoluteError);
        var L2 = crossValidationResults.Select(r => r.Metrics.MeanSquaredError);
        var RMS = crossValidationResults.Select(r => r.Metrics.RootMeanSquaredError);
        var lossFunction = crossValidationResults.Select(r => r.Metrics.LossFunction);
        var R2 = crossValidationResults.Select(r => r.Metrics.RSquared);

        Log.Logger.Information($"*************************************************************************************************************");
        Log.Logger.Information($"*       Metrics for {algorithmName} Regression model      ");
        Log.Logger.Information($"*------------------------------------------------------------------------------------------------------------");
        Log.Logger.Information($"*       Average L1 Loss:    {L1.Average():0.###} ");
        Log.Logger.Information($"*       Average L2 Loss:    {L2.Average():0.###}  ");
        Log.Logger.Information($"*       Average RMS:          {RMS.Average():0.###}  ");
        Log.Logger.Information($"*       Average Loss Function: {lossFunction.Average():0.###}  ");
        Log.Logger.Information($"*       Average R-squared: {R2.Average():0.###}  ");
        Log.Logger.Information($"*************************************************************************************************************");
    }

    public static void PrintMulticlassClassificationFoldsAverageMetrics(
        string algorithmName,
        IReadOnlyList<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> crossValResults
    )
    {
        var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

        var microAccuracyValues = metricsInMultipleFolds.Select(m => m.MicroAccuracy);
        var microAccuracyAverage = microAccuracyValues.Average();
        var microAccuraciesStdDeviation = CalculateStandardDeviation(microAccuracyValues);
        var microAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(microAccuracyValues);

        var macroAccuracyValues = metricsInMultipleFolds.Select(m => m.MacroAccuracy);
        var macroAccuracyAverage = macroAccuracyValues.Average();
        var macroAccuraciesStdDeviation = CalculateStandardDeviation(macroAccuracyValues);
        var macroAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(macroAccuracyValues);

        var logLossValues = metricsInMultipleFolds.Select(m => m.LogLoss);
        var logLossAverage = logLossValues.Average();
        var logLossStdDeviation = CalculateStandardDeviation(logLossValues);
        var logLossConfidenceInterval95 = CalculateConfidenceInterval95(logLossValues);

        var logLossReductionValues = metricsInMultipleFolds.Select(m => m.LogLossReduction);
        var logLossReductionAverage = logLossReductionValues.Average();
        var logLossReductionStdDeviation = CalculateStandardDeviation(logLossReductionValues);
        var logLossReductionConfidenceInterval95 = CalculateConfidenceInterval95(logLossReductionValues);

        Log.Logger.Information($"*************************************************************************************************************");
        Log.Logger.Information($"*       Metrics for {algorithmName} Multi-class Classification model      ");
        Log.Logger.Information($"*------------------------------------------------------------------------------------------------------------");
        Log.Logger.Information($"*       Average MicroAccuracy:    {microAccuracyAverage:0.###}  - Standard deviation: ({microAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({microAccuraciesConfidenceInterval95:#.###})");
        Log.Logger.Information($"*       Average MacroAccuracy:    {macroAccuracyAverage:0.###}  - Standard deviation: ({macroAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({macroAccuraciesConfidenceInterval95:#.###})");
        Log.Logger.Information($"*       Average LogLoss:          {logLossAverage:#.###}  - Standard deviation: ({logLossStdDeviation:#.###})  - Confidence Interval 95%: ({logLossConfidenceInterval95:#.###})");
        Log.Logger.Information($"*       Average LogLossReduction: {logLossReductionAverage:#.###}  - Standard deviation: ({logLossReductionStdDeviation:#.###})  - Confidence Interval 95%: ({logLossReductionConfidenceInterval95:#.###})");
        Log.Logger.Information($"*************************************************************************************************************");

    }

    public static double CalculateStandardDeviation(IEnumerable<double> values)
    {
        var average = values.Average();
        var sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
        var standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
        return standardDeviation;
    }

    public static double CalculateConfidenceInterval95(IEnumerable<double> values)
    {
        var confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt(values.Count() - 1);
        return confidenceInterval95;
    }

    public static void PrintClusteringMetrics(string name, ClusteringMetrics metrics)
    {
        Log.Logger.Information($"*************************************************");
        Log.Logger.Information($"*       Metrics for {name} clustering model      ");
        Log.Logger.Information($"*------------------------------------------------");
        Log.Logger.Information($"*       Average Distance: {metrics.AverageDistance}");
        Log.Logger.Information($"*       Davies Bouldin Index is: {metrics.DaviesBouldinIndex}");
        Log.Logger.Information($"*************************************************");
    }

    public static void ShowDataViewInConsole(MLContext mlContext, IDataView dataView, int numberOfRows = 4)
    {
        var msg = $"Show data in DataView: Showing {numberOfRows.ToString()} rows with the columns";
        WriteHeader(msg);

        var preViewTransformedData = dataView.Preview(maxRows: numberOfRows);

        foreach (var row in preViewTransformedData.RowView)
        {
            var columnCollection = row.Values;
            var lineToPrint = "Row--> ";
            foreach (var column in columnCollection)
            {
                lineToPrint += $"| {column.Key}:{column.Value}";
            }
            Log.Logger.Information(lineToPrint + "\n");
        }
    }

    [Conditional("DEBUG")]
    // This method using 'DebuggerExtensions.Preview()' should only be used when debugging/developing, not for release/production trainings
    public static void PeekDataViewInConsole(MLContext mlContext, IDataView dataView, IEstimator<ITransformer> pipeline, int numberOfRows = 4)
    {
        var msg = $"Peek data in DataView: Showing {numberOfRows.ToString()} rows with the columns";
        WriteHeader(msg);

        //https://github.com/dotnet/machinelearning/blob/main/docs/code/MlNetCookBook.md#how-do-i-look-at-the-intermediate-data
        var transformer = pipeline.Fit(dataView);
        var transformedData = transformer.Transform(dataView);

        // 'transformedData' is a 'promise' of data, lazy-loading. call Preview
        //and iterate through the returned collection from preview.

        var preViewTransformedData = transformedData.Preview(maxRows: numberOfRows);

        foreach (var row in preViewTransformedData.RowView)
        {
            var ColumnCollection = row.Values;
            var lineToPrint = "Row--> ";
            foreach (var column in ColumnCollection)
            {
                lineToPrint += $"| {column.Key}:{column.Value}";
            }
            Log.Logger.Information(lineToPrint + "\n");
        }
    }

    [Conditional("DEBUG")]
    // This method using 'DebuggerExtensions.Preview()' should only be used when debugging/developing, not for release/production trainings
    public static void PeekVectorColumnDataInConsole(MLContext mlContext, string columnName, IDataView dataView, IEstimator<ITransformer> pipeline, int numberOfRows = 4)
    {
        var msg = $"Peek data in DataView: : Show {numberOfRows} rows with just the '{columnName}' column";
        WriteHeader(msg);

        var transformer = pipeline.Fit(dataView);
        var transformedData = transformer.Transform(dataView);

        // Extract the 'Features' column.
        var someColumnData = transformedData.GetColumn<float[]>(columnName).Take(numberOfRows).ToList();

        // print to console the peeked rows

        var currentRow = 0;
        someColumnData.ForEach(row =>
        {
            currentRow++;
            var concatColumn = string.Empty;
            foreach (var f in row)
            {
                concatColumn += f.ToString();
            }

            Log.Logger.Information("");
            var rowMsg = string.Format("**** Row {0} with '{1}' field value ****", currentRow, columnName);
            Log.Logger.Information(rowMsg);
            Log.Logger.Information(concatColumn);
            Log.Logger.Information("");
        });
    }

    public static void WriteHeader(params string[] lines)
    {
        Log.Logger.Information(" ");
        foreach (var line in lines)
        {
            Log.Logger.Information(line);
        }
        var maxLength = lines.Select(x => x.Length).Max();
        Log.Logger.Information(new string('#', maxLength));
    }

    public static void WriterSection(params string[] lines)
    {
        Log.Logger.Information(" ");
        foreach (var line in lines)
        {
            Log.Logger.Information(line);
        }
        var maxLength = lines.Select(x => x.Length).Max();
        Log.Logger.Information(new string('-', maxLength));
    }

    public static void SignalFinished()
    {
        Log.Logger.Information(" ");
        Log.Logger.Information("Finished.");
    }

    public static void WriteException(params string[] lines)
    {
        const string exceptionTitle = "EXCEPTION";
        Log.Logger.Information(" ");
        Log.Logger.Information(exceptionTitle);
        Log.Logger.Information(new string('#', exceptionTitle.Length));
        foreach (var line in lines)
        {
            Log.Logger.Error(line);
        }
    }

    public static void WriteWarning(params string[] lines)
    {
        const string warningTitle = "WARNING";
        Log.Logger.Information(" ");
        Log.Logger.Information(warningTitle);
        Log.Logger.Information(new string('#', warningTitle.Length));
        foreach (var line in lines)
        {
            Log.Logger.Warning(line);
        }
    }
}
