using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FinanceSaldo.View.Extensions
{
    public class GrayoutImageBehavior
    {
        public static readonly DependencyProperty GrayOutOnDisabledProperty = DependencyProperty.RegisterAttached("GrayOutOnDisabled", typeof(bool), typeof(GrayoutImageBehavior), new PropertyMetadata(default(bool), OnGrayOutOnDisabledChanged));
        public static void SetGrayOutOnDisabled(Image element, bool value) { element.SetValue(GrayOutOnDisabledProperty, value); }
        public static bool GetGrayOutOnDisabled(Image element) { return (bool)element.GetValue(GrayOutOnDisabledProperty); }

        private static void OnGrayOutOnDisabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Image image = (Image)obj;
            image.IsEnabledChanged -= OnImageIsEnabledChanged;

            if ((bool)args.NewValue) image.IsEnabledChanged += OnImageIsEnabledChanged;

            ToggleGrayOut(image); // initial call
        }

        private static void OnImageIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            Image image = (Image)sender;
            ToggleGrayOut(image);
        }

        private static void ToggleGrayOut(Image image)
        {
            try
            {
                if (image.IsEnabled)
                {
                    if (!(image.Source is FormatConvertedBitmap grayImage)) return;
                    image.Source = grayImage.Source; // Set the Source property to the original value.
                    image.OpacityMask = null; // Reset the Opacity Mask
                    image.Opacity = 1.0;
                }
                else
                {
                    BitmapImage bitmapImage = default(BitmapImage);

                    switch (image.Source)
                    {
                        case BitmapImage _:
                            bitmapImage = (BitmapImage)image.Source;
                            break;
                        case BitmapSource _:
                            bitmapImage = new BitmapImage(new Uri(image.Source.ToString()));
                            break;
                    }

                    if (bitmapImage == null) return;
                    image.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0); // Get the source bitmap
                    // Create Opacity Mask for grayscale image as FormatConvertedBitmap does not keep transparency info
                    image.OpacityMask = new ImageBrush(bitmapImage); 
                    image.Opacity = 0.3; // optional: lower opacity
                }
            }
            catch (Exception ex)
            {
                //LogicLogger.WriteLogEntry("Converting image to grayscale failed", LogLevel.Debug, false, ex);
            }
        }
    }
}
