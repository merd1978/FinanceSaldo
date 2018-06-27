using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FinanceSaldo.View.Extensions
{
    public class TextBoxInputBehavior : Behavior<TextBox>
    {
        const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint |
                                                   NumberStyles.AllowThousands |
                                                   NumberStyles.AllowLeadingSign;
        public TextBoxInputBehavior()
        {
            this.InputMode = TextBoxInputMode.None;
            this.JustPositivDecimalInput = false;
        }

        public TextBoxInputMode InputMode { get; set; }


        public static readonly DependencyProperty JustPositivDecimalInputProperty =
         DependencyProperty.Register("JustPositivDecimalInput", typeof(bool),
         typeof(TextBoxInputBehavior), new FrameworkPropertyMetadata(false));

        public bool JustPositivDecimalInput
        {
            get => (bool)GetValue(JustPositivDecimalInputProperty);
            set => SetValue(JustPositivDecimalInputProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                if (this.IsValidInput(this.GetText(pastedText))) return;
                System.Media.SystemSounds.Beep.Play();
                e.CancelCommand();
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                e.CancelCommand();
            }
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var txt = AssociatedObject;
            int CaretIndex = txt.CaretIndex;

            switch (e.Key)
            {
                case Key.OemPeriod:
                case Key.Decimal:
                    // If the user typed a period and we're to the left of the decimal point, skip over the decimal point
                    if (CaretIndex < txt.Text.Length && txt.Text[CaretIndex] == ',')
                    {
                        txt.CaretIndex++;
                        e.Handled = true;
                    }
                    // Or if there is already a decimal point, don't add a second one
                    // (If user is replacing the part that includes the decimal point, accept it)
                    else if (txt.Text.Contains(".") && !txt.SelectedText.Contains(","))
                    {
                        e.Handled = true;
                    }

                    break;

                case Key.Back:
                    // If the user pressed Backspace and we're to the right of the only decimal point, skip over the decimal point
                    if (CaretIndex > 0 && CaretIndex <= txt.Text.Length && txt.Text[CaretIndex - 1] == ',' && HasOnlyOne(txt.Text, ','))
                    {
                        txt.CaretIndex--;
                        e.Handled = true;
                    }

                    break;

                case Key.Delete:
                    // If the user pressed Delete and we're to the left of the only decimal point, skip over the decimal point
                    if (CaretIndex < txt.Text.Length && txt.Text[CaretIndex] == ',' && HasOnlyOne(txt.Text, ','))
                    {
                        txt.CaretIndex++;
                        e.Handled = true;
                    }

                    break;
            }

            if (e.Key != Key.Space) return;
            if (IsValidInput(GetText(" "))) return;
            System.Media.SystemSounds.Beep.Play();
            e.Handled = true;
        }

        private static bool HasOnlyOne(string text, char character)
        {
            int count = 0;
            foreach (var ch in text)
            {
                if (ch == character)
                {
                    if (count == 1) return false;
                    count++;
                }
            }
            return count == 1;
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!this.IsValidInput(this.GetText(e.Text)))
            {
                System.Media.SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        private string GetText(string input)
        {
            var txt = AssociatedObject;

            int selectionStart = txt.SelectionStart;
            if (txt.Text.Length < selectionStart)
                selectionStart = txt.Text.Length;

            int selectionLength = txt.SelectionLength;
            if (txt.Text.Length < selectionStart + selectionLength)
                selectionLength = txt.Text.Length - selectionStart;

            var realtext = txt.Text.Remove(selectionStart, selectionLength);

            int caretIndex = txt.CaretIndex;
            if (realtext.Length < caretIndex)
                caretIndex = realtext.Length;

            var newtext = realtext.Insert(caretIndex, input);

            return newtext;
        }

        private bool IsValidInput(string input)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;
                case TextBoxInputMode.DigitInput:
                    return CheckIsDigit(input);

                case TextBoxInputMode.DecimalInput:
                    //wen mehr als ein Komma
                    int pointCount = 0;
                    foreach (var x in input.ToCharArray())
                    {
                        if (x == ',')
                        {
                            pointCount++;
                        }
                    }
                    if (pointCount > 1) return false;

                    //block input extra digit after last digit in formatted string with decimal point
                    var txt = AssociatedObject;
                    int caretIndex = txt.CaretIndex;
                    if (pointCount == 1 && caretIndex == txt.Text.Length)
                    {
                        return false;
                    }

                    if (input.Contains("-"))
                    {
                        if (JustPositivDecimalInput)
                            return false;

                        if (input.IndexOf("-", StringComparison.Ordinal) > 0)
                            return false;

                        if (input.ToCharArray().Count(x => x == '-') > 1)
                            return false;

                        //minus einmal am anfang zulässig
                        if (input.Length == 1)
                            return true;
                    }

                    var result = decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out _);
                    return result;

                default: throw new ArgumentException("Unknown TextBoxInputMode");
            }
        }

        private static bool CheckIsDigit(string wert)
        {
            return wert.ToCharArray().All(char.IsDigit);
        }
    }

    public enum TextBoxInputMode
    {
        None,
        DecimalInput,
        DigitInput
    }
}
