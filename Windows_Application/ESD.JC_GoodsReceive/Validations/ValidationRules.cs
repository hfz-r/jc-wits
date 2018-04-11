using ESD.JC_GoodsReceive.ModelsExt;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ESD.JC_GoodsReceive.Validations
{
    [ContentProperty("ComparisonValue")]
    public class RowDataInfoValidationRule : ValidationRule
    {
        public ComparisonValue ComparisonValue { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            BindingGroup group = (BindingGroup)value;
            var datas = ComparisonValue.Collection as ObservableCollection<EunKGExt>;

            StringBuilder error = null;
            foreach (var item in group.Items)
            {
                var data = item as EunKGExt;
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.Error))
                    {
                        if (error == null)
                            error = new StringBuilder();
                        error.Append((error.Length != 0 ? ", " : "") + data.Error);
                    }
                    if ((data.Qty > ComparisonValue.Value) || (datas.Sum(x => x.Qty) > ComparisonValue.Value))
                    {
                        if (error == null)
                            error = new StringBuilder();
                        error.Append((error.Length != 0 ? ", " : "") + string.Format("Child quantity should be LESS than {0}", ComparisonValue.Value));
                    }
                }
            }

            if (error != null)
                return new ValidationResult(false, error.ToString());

            return ValidationResult.ValidResult;
        }
    }
}
