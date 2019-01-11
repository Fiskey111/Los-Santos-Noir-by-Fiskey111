using System.Collections.Generic;

namespace LSNoir.CaseDataValidation
{
    class ValidationResult
    {
        public string Title { get; }

        public List<string> Warnings { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
        public List<string> Info { get; } = new List<string>();

        public List<ValidationResult> Children { get; } = new List<ValidationResult>();

        public ValidationResult(string reportTitle)
        {
            Title = reportTitle;
        }

        public void AddWarning(string caseId, string stageId, string resId, string txt)
        {
            AddToList(Warnings, "WARNING", caseId, stageId, resId, txt);
        }

        public void AddError(string caseId, string stageId, string resId, string txt)
        {
            AddToList(Errors, "ERROR", caseId, stageId, resId, txt);
        }

        public void AddInfo(string caseId, string stageId, string resId, string txt)
        {
            AddToList(Info, "INFO", caseId, stageId, resId, txt);
        }

        private static void AddToList(List<string> list, string msgType, string caseId, string stageId, string resId, string txt)
        {
            list.Add($"{msgType}: Case: [{caseId}] stage: [{stageId}] resource: [{resId}]: txt");
        }

        public void AddChildReport(ValidationResult child)
        {
            Children.Add(child);
        }

        public string GetReport()
        {
            return "";
        }
    }
}
