using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WinFred.ValidationRules
{
    class FileExtensionValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var bindingGroup = value as BindingGroup;
            if (bindingGroup != null)
            {
                FileExtension fileExtension = bindingGroup.Items[0] as FileExtension;
                if (fileExtension != null && (fileExtension.Priority < -1 || fileExtension.Priority > 10000))
                {
                    return new ValidationResult(false, "Priority must be inside the range ]-1, 10000[");
                }
                if (fileExtension.Extension.Contains("."))
                {
                    return new ValidationResult(false, "Extension must not contain a dot");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
