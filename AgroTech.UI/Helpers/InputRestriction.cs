using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AgroTech.UI.Helpers
{
    public enum RestrictionType
    {
        None,
        LettersOnly,
        NumbersOnly,
        Alphanumeric,
        EmailChars
    }

    public static class InputRestriction
    {
        public static readonly DependencyProperty RestrictionProperty =
            DependencyProperty.RegisterAttached(
                "Restriction",
                typeof(RestrictionType),
                typeof(InputRestriction),
                new PropertyMetadata(RestrictionType.None, OnRestrictionChanged));

        public static RestrictionType GetRestriction(DependencyObject obj)
        {
            return (RestrictionType)obj.GetValue(RestrictionProperty);
        }

        public static void SetRestriction(DependencyObject obj, RestrictionType value)
        {
            obj.SetValue(RestrictionProperty, value);
        }

        private static void OnRestrictionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler(textBox, TextBox_Pasting);

                if ((RestrictionType)e.NewValue != RestrictionType.None)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    DataObject.AddPastingHandler(textBox, TextBox_Pasting);
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var restriction = GetRestriction(textBox);
                e.Handled = !IsValid(e.Text, restriction);
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (sender is TextBox textBox)
                {
                    var restriction = GetRestriction(textBox);
                    if (!IsValid(text, restriction))
                    {
                        e.CancelCommand();
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsValid(string text, RestrictionType restriction)
        {
            return restriction switch
            {
                RestrictionType.LettersOnly => Regex.IsMatch(text, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$"),
                RestrictionType.NumbersOnly => Regex.IsMatch(text, @"^[0-9,.]+$"), // Permitir decimales en dimensiones
                RestrictionType.Alphanumeric => Regex.IsMatch(text, @"^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚ\s]+$"),
                RestrictionType.EmailChars => Regex.IsMatch(text, @"^[a-zA-Z0-9@._\-]+$"), // Permitir caracteres de correo
                _ => true,
            };
        }
    }
}
