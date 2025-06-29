// клас за консумиране на модела

using Forum.MLModel;
using Microsoft.ML;
using System.IO;

namespace SentimentAnalysis 
{
    public static class SentimentAnalysis 
    {
        private static readonly string _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModel", "SentimentModel.zip");
        private static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> _predictionEngine = new(() =>
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(_modelPath, out _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        });

        public static ModelOutput Predict(ModelInput input)
        {
            return _predictionEngine.Value.Predict(input);
        }
    }

    public class ModelInput
    {
        public string SentimentText { get; set; } = string.Empty;
    }

    public class ModelOuput
    {
        public bool PredictedLabel { get; set; }
        public float[] Score { get; set; } = [];
    }
}