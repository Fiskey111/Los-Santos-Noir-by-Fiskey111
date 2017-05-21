using LSNoir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.CaseDataValidation
{
    //TODO:
    // - use consolecmd to run a test on a selected case
    class CaseDataValidator : IValidator
    {
        private readonly CaseData data;

        public CaseDataValidator(CaseData caseData)
        {
            data = caseData;
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult("");

            if(string.IsNullOrEmpty(data.ID))
            {
                result.AddError("", "", nameof(data.ID), "Case has no ID, full analysis could not be performed!");
                return result;
            }

            if(string.IsNullOrEmpty(data.Name))
            {
                result.AddError(data.ID, "", nameof(data.Name), "Case name is empty");
            }

            if(data.Stages == null || data.Stages.Length < 1)
            {
                result.AddError(data.ID, "", "StagesID array", "Case has no assigned stages");
            }
            else
            {
                foreach (var stage in data.Stages)
                {
                    var stageData = data.GetStageData(stage);
                    if (stageData == null)
                    {
                        result.AddError(data.ID, stage, "", "StageData with given ID could not be found");
                    }
                    else
                    {
                        var stageValidator = new StageDataValidator(stageData);
                        var stageAnalysis = stageValidator.Validate();
                        result.AddChildReport(stageValidator.Validate());
                    }
                }
            }

            if (string.IsNullOrEmpty(data.Address))
            {
                result.AddWarning(data.ID, "", nameof(data.Address), "Address is empty");
            }

            if(string.IsNullOrEmpty(data.City))
            {
                result.AddWarning(data.ID, "", nameof(data.City), "City is empty");
            }

            if(string.IsNullOrEmpty(data.FirstOfficer))
            {
                result.AddWarning(data.ID, "", nameof(data.FirstOfficer), "First officer is empty");
            }


            return result;
        }
    }
}
