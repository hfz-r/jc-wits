using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ESD.JC_LabelPrinting.Views
{
    public class TextBoxOnEscapeBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            if (this.AssociatedObject != null)
            {
                base.OnAttached();
                this.AssociatedObject.Loaded += (sender, args) => SetMaxLength();
                this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            }
        }

        protected override void OnDetaching()
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Loaded -= (sender, args) => SetMaxLength();
                this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
                base.OnDetaching();
            }
        }

        private void SetMaxLength()
        {
            object context = AssociatedObject.DataContext;
            BindingExpression binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);

            if (context != null && binding != null)
            {
                PropertyInfo prop = context.GetType().GetProperty(binding.ParentBinding.Path.Path);
                if (prop != null)
                {
                    var att = prop.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault() as StringLengthAttribute;
                    if (att != null)
                    {
                        AssociatedObject.MaxLength = att.MaximumLength;
                    }
                }
            }
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Escape)
            {
                FrameworkElement parent = (FrameworkElement)textBox.Parent;
                while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
                {
                    parent = (FrameworkElement)parent.Parent;
                }

                DependencyObject scope = FocusManager.GetFocusScope(textBox);
                FocusManager.SetFocusedElement(scope, parent as IInputElement);
            }
        }
    }
}
