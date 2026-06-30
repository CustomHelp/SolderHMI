using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace LOET_HMI.UserControls
{
    // https://putridparrot.com/blog/basics-of-extending-a-wpf-control/



    public class TextBoxNewRecipe : TextBox
    {
        //Step 2/a :
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(TextBoxNewRecipe), new FrameworkPropertyMetadata(String.Empty));

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        //Step 2/b :
        private static readonly DependencyPropertyKey RemoveWatermarkPropertyKey =
            DependencyProperty.RegisterReadOnly("RemoveWatermark", typeof(bool), typeof(TextBoxNewRecipe), new FrameworkPropertyMetadata((bool)false));

        public static readonly DependencyProperty RemoveWatermarkProperty = RemoveWatermarkPropertyKey.DependencyProperty;

        public bool RemoveWatermark
        {
            get { return (bool)GetValue(RemoveWatermarkProperty); }
        }



        //Step 2/d:
        static void TextPropertyChanged(DependencyObject sender,  DependencyPropertyChangedEventArgs args)
        {
            TextBoxNewRecipe watermarkTextBox = (TextBoxNewRecipe)sender;

            bool textExists = watermarkTextBox.Text.Length > 0;
            if (textExists != watermarkTextBox.RemoveWatermark)
            {
                watermarkTextBox.SetValue(RemoveWatermarkPropertyKey, textExists);
            }
        }





        // Step 1:
        static TextBoxNewRecipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxNewRecipe), new FrameworkPropertyMetadata(typeof(TextBoxNewRecipe)));


            //Step 2/c :
            TextProperty.OverrideMetadata(typeof(TextBoxNewRecipe), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
        }


    }

}
