using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WinFred.Search;

namespace WinFred.ValidationRules
{
    public class FileExtensionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var bindingGroup = value as BindingGroup;
            if (bindingGroup != null)
            {
                var fileExtension = bindingGroup.Items[0] as FileExtension;
                if (fileExtension != null && (fileExtension.Priority < -1 || fileExtension.Priority > 10000))
                {
                    return new ValidationResult(false, "Priority must be inside the range ]-1, 10000[");
                }
                if (fileExtension.Extension != null && fileExtension.Extension.Contains("."))
                {
                    return new ValidationResult(false, "Extension must not contain a dot");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
    public class FileExtensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var extension = (string)value;
                if (!extension.Contains("."))
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}