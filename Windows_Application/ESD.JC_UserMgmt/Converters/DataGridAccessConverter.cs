using DataLayer.Repositories;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace ESD.JC_UserMgmt.Converters
{
    public class DataGridAccessConverter : IMultiValueConverter
    {
        private IRoleRepository roleRepository = new RoleRepository();

        string AuthenticatedUser = Thread.CurrentPrincipal.Identity.Name;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null)
            {
                if (values[0] == DependencyProperty.UnsetValue)
                    return false;

                if ((long)values[0] != 0)
                {
                    if (values[1] != null)
                    {
                        if (values[1] == DependencyProperty.UnsetValue)
                            return false;
                        if (AuthenticatedUser == "sa")
                            return false;
                        if ((string)values[1] == AuthenticatedUser)
                        {
                            if (values[2] != null)
                            {
                                if (values[2] == DependencyProperty.UnsetValue)
                                    return false;
                                var role = roleRepository.GetRole((long)values[2]).RoleCode;
                                if (role == "ADMINISTRATOR")
                                    return false;
                            }
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                  
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
