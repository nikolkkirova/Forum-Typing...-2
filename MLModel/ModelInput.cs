namespace Forum.MLModel
{
    public class ModelInput
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ModelOutput
    {
        public bool PredictedLabel { get; set; } // true --> позитивен, false --> негативен
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}