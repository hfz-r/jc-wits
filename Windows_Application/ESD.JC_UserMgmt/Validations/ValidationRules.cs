using ESD.JC_UserMgmt.ModelsExt;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ESD.JC_UserMgmt.Validations
{
    [ContentProperty("ComparisonValue")]
    public class UserDataGrid : ValidationRule
    {
        public ComparisonValue ComparisonValue { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var user = (value as BindingGroup).Items[0] as UserExt;
            var lstCollection = ComparisonValue.Collection as ObservableCollection<UserExt>;

            if (user != null)
            {
                if (lstCollection.Where(nm => nm.Username == user.Username &&
                                        !string.IsNullOrEmpty(user.Username)).Count() > 1)
                {
                    return new ValidationResult(false, $"{user.Username} already Exist! Please find another.");
                }
                if (lstCollection.Where(nm => nm.Email == user.Email &&
                                        !string.IsNullOrEmpty(user.Email)).Count() > 1)
                {
                    return new ValidationResult(false, $"{user.Email} already Exist! Please find another.");
                }

                var usernameflag = lstCollection.GroupBy(g => g.Username).Where(c => c.Count() > 1).Select(x => x.Key);
                if (usernameflag.Count() > 0 && !string.IsNullOrEmpty(usernameflag.First()))
                {
                    return new ValidationResult(false, $"{usernameflag.First()} already Exist! Please find another.");
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
