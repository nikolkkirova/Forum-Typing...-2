using Microsoft.ML;
using Forum.MLModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Forum.MLModel
{
    public class SentimentService // клас, който предоставя функционалност за предсказване на настроението на коментар
    {
        private readonly MLContext _mlContext; // създава се контекст за всички ML операции
        private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine; // обект, който извършва предсказването

        public SentimentService() 
        {
            _mlContext = new MLContext();

            // път до модела
            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModel", "SentimentModel.zip");

            // зарежда предварително обучен модел от .zip файла
            ITransformer mlModel = _mlContext.Model.Load(modelPath, out _);

            // тук се създава PredictionEngine, който ще предсказва на базата на входен текст
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        public ModelOutput Predict(string text) // метод Predict!!! приема текст и връща резултат от предсказването
        {
            var input = new ModelInput { Text = text }; // обект с входен текст, който да се подаде към PredictionEngine
            return _predictionEngine.Predict(input);
        }
    }
}