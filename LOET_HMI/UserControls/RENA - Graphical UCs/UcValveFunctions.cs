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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOET_HMI
{
    public static class UcValveFunctions
    {
        public static void AdjustGeometry(  Polygon         polygonLeft, 
                                            Polygon         polygonRight, 
                                            Grid            grid, 
                                            RowDefinition   row0, 
                                            Ellipse         centerCircle,
                                            double          length)
        {
            polygonLeft.Points.Clear();
            polygonRight.Points.Clear();


            polygonLeft.Points.Add(new Point(0, 0));
            polygonLeft.Points.Add(new Point(0, length));
            polygonLeft.Points.Add(new Point(length, length / 2));

            polygonRight.Points.Add(new Point(0, 0));
            polygonRight.Points.Add(new Point(0, length));
            polygonRight.Points.Add(new Point(length, length / 2));

            polygonRight.RenderTransform = new RotateTransform(180, length / 2, length / 2);

            grid.Width = length;
            grid.Height = length;
            row0.Height = new GridLength(length / 2 + 2);
            centerCircle.Height = length / 2;
            centerCircle.Width = length / 2;
        }

        public static void UcLoaded(    Grid mainGrid,
                                        RowDefinition row0,
                                        RowDefinition row1)
        {
            double valveCenter = row0.ActualHeight + row1.ActualHeight;
            double offsetToCenter = mainGrid.ActualHeight / 2 - valveCenter;

            mainGrid.Height = mainGrid.ActualHeight;
            mainGrid.Width = mainGrid.ActualWidth;

            mainGrid.Margin = new Thickness(0, offsetToCenter, 0, 0);



        }


    }
}
