using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Graph_Manager.ViewModel
{
    public class ValidationClass : ValidationRule
    {
        public override ValidationResult Validate
          (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!Validation.IsEven(value))
                return new ValidationResult(false, "Nie można storzyć takiego grafu!");

            if (!Validation.IsPruferCode(value))
                return new ValidationResult(false, "Kod jest niepoprawny!");


            return ValidationResult.ValidResult;
        }
    }
}
