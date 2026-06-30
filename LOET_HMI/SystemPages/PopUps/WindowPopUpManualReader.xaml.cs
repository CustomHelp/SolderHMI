using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpManual.xaml
    /// </summary>
    public partial class WindowPopUpManualReader : Window
    {
        private string path;
        private bool twoPages = true;
        private int page = 0;
        private int maxPages = 0;
        private FileInfo[] files;
        public WindowPopUpManualReader(string filename)
        {
            InitializeComponent();
            path = filename + "\\";
            DirectoryInfo d = new DirectoryInfo(path);
            files = d.GetFiles();
            maxPages = files.Count();
            //if (filename.Contains("MANUAL"))
            //{
            //    SetUpWindowManual();
            //    twoPages = true;
            //}
            //if (filename.Contains("CIRCUIT DIAGRAM"))
            //{
            SetUpWindowCircuitDiagram();
            twoPages = false;
            //}

        }

        public void SetUpWindowManual()
        {
            firstPage.Source = new BitmapImage(new Uri(files[page].FullName));
            secondPage.Source = new BitmapImage(new Uri(files[page + 1].FullName));
        }

        public void SetUpWindowCircuitDiagram()
        {
            firstPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            firstPage.VerticalAlignment = VerticalAlignment.Stretch;
            firstPage.Margin = new Thickness(40, 10, 40, 10);
            Grid.SetColumnSpan(firstPage, 2);
            secondPage.Visibility = Visibility.Collapsed;
            if (files.Count() > 0)
            {
                firstPage.Source = new BitmapImage(new Uri(files[page].FullName));
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (twoPages == true)
            {
                if (page < maxPages - 2)
                {
                    page += 2;
                    firstPage.Source = new BitmapImage(new Uri(files[page].FullName));
                    if (page + 1 < maxPages)
                    {
                        secondPage.Source = new BitmapImage(new Uri(files[page + 1].FullName));
                    }
                    else
                    {
                        secondPage.Source = null;
                    }
                }
            }
            else
            {
                if (page < maxPages - 1)
                {
                    page++;
                    firstPage.Source = new BitmapImage(new Uri(files[page].FullName));
                }
            }
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (twoPages == true)
            {
                if (page >= 2)
                {
                    page -= 2;
                    firstPage.Source = new BitmapImage(new Uri(files[page].FullName));
                    secondPage.Source = new BitmapImage(new Uri(files[page + 1].FullName));
                }
            }
            else
            {
                if (page > 0)
                {
                    page--;
                    firstPage.Source = new BitmapImage(new Uri(files[page].FullName));
                }
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
