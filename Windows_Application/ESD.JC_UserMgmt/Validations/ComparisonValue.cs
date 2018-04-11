using System.Windows;

namespace ESD.JC_UserMgmt.Validations
{
    public class ComparisonValue : DependencyObject
    {
        public object Collection
        {
            get { return GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(
                nameof(Collection),
                typeof(object),
                typeof(ComparisonValue),
                new FrameworkPropertyMetadata(default(object)));

    }
}
