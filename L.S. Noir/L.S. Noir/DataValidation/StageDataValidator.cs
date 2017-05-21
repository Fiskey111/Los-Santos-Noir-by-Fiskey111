using LSNoir.Data;

namespace LSNoir.CaseDataValidation
{
    class StageDataValidator : IValidator
    {
        private readonly StageData data;

        public StageDataValidator(StageData stageData)
        {
            data = stageData;
        }

        public ValidationResult Validate()
        {
            return new ValidationResult($"StageData analysis: {data.ID}");
        }
    }
}
