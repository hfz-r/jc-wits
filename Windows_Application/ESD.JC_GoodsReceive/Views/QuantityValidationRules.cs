using ESD.JC_GoodsReceive.ModelsExt;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ESD.JC_GoodsReceive.Views
{
    [ContentProperty("ComparisonValue")]
    public class QuantityValidationRules : ValidationRule
    {
        public ComparisonValue ComparisonValue { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            EunKGExt kg = (value as BindingGroup).Items[0] as EunKGExt;

            ObservableCollection<EunKGExt> kgs = ComparisonValue.Collection as ObservableCollection<EunKGExt>;

            if (kg.Qty > ComparisonValue.Value)
            {
                return new ValidationResult(false, $"Child quantity should be LESS than {ComparisonValue.Value}");
            }
            if (kgs.Sum(x => x.Qty) > ComparisonValue.Value)
            {
                return new ValidationResult(false, $"Child quantity should be LESS than {ComparisonValue.Value}");
            }

            return ValidationResult.ValidResult;
        }
    }
}
