using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.CaseDataValidation
{
    interface IValidator
    {
        ValidationResult Validate();
    }
}
