using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcContainer.xaml
    /// </summary>
    /// 


    // Quelle: https://stackoverflow.com/questions/5758342/how-to-create-wpf-usercontrol-which-contains-placeholders-for-later-usage

    public partial class UcContainer : UserControl
    {
        /*
        public static readonly DependencyProperty ContainerHeaderProperty =
        DependencyProperty.Register("ContainerHeader", typeof(object), typeof(UcContainer), new UIPropertyMetadata(null));
        public object ContainerHeader
        {
            get { return (object)GetValue(ContainerHeaderProperty); }
            set { SetValue(ContainerHeaderProperty, value); }
        }
        */

        public string sHeaderContent { get; set; }



        public static readonly DependencyProperty PlaceHolder1Property =
                DependencyProperty.Register("PlaceHolder1", typeof(object), typeof(UcContainer), new UIPropertyMetadata(null));
        public object PlaceHolder1
        {
            get { return (object)GetValue(PlaceHolder1Property); }
            set { SetValue(PlaceHolder1Property, value); }
        }


        public UcContainer()
        {
            InitializeComponent();
            this.DataContext = this;
            sHeaderContent = "...set the 'sHeaderContent' property ...";

        }
    }
}
