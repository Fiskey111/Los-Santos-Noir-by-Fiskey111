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
            var result = new ValidationResult($"Stage validation result: {data.ID}");
            if(string.IsNullOrEmpty(data.StageType))
            {
                result.AddError(data.ParentCase.ID, data.ID, "", "Stage has no type.");
            }

            if(data.Resources == null || data.Resources.Length < 1)
            {
                result.AddInfo(data.ParentCase.ID, data.ID, "", "Stage has no resources.");
            }
            else
            {
                foreach (var res in data.Resources)
                {
                }
            }

            return new ValidationResult($"StageData analysis: {data.ID}");
        }
    }
}
