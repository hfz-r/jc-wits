using System.Windows;
using System.Windows.Data;

namespace ESD.JC_GoodsReceive.Views
{
    public class ComparisonValue : DependencyObject
    {
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public object Collection
        {
            get { return GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(decimal),
                typeof(ComparisonValue),
                new PropertyMetadata(default(decimal)));

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(
                nameof(Collection),
                typeof(object),
                typeof(ComparisonValue),
                new FrameworkPropertyMetadata(default(object)));

    }
}
