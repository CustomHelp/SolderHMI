using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PdfiumViewer;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpPdfReader.xaml
    /// Echtes PDF-Rendering (PdfiumViewer / Google PDFium) als Bild pro Seite.
    /// Read-only, mit Zoom, Blättern und Sprung zu einer Seite. Touch-tauglich.
    /// Ersetzt den alten bildserien-basierten WindowPopUpManualReader.
    /// </summary>
    public partial class WindowPopUpPdfReader : Window
    {
        private const float BaseDpi = 96f;
        private const double ZoomStep = 1.25;
        private const double MinZoom = 0.25;
        private const double MaxZoom = 6.0;

        private PdfDocument document;
        private int currentPage = 0;
        private double zoom = 1.0;

        // Drag-Panning (Touch ist global als Maus promotet -> Maus-Drag verschiebt den Ausschnitt).
        private bool isPanning;
        private System.Windows.Point panStartPoint;
        private double panStartHOffset;
        private double panStartVOffset;

        public WindowPopUpPdfReader(string filePath)
        {
            InitializeComponent();

            // Drag-Panning auf dem ScrollViewer aktivieren.
            scrollViewer.PreviewMouseLeftButtonDown += ScrollViewer_PreviewMouseLeftButtonDown;
            scrollViewer.PreviewMouseMove += ScrollViewer_PreviewMouseMove;
            scrollViewer.PreviewMouseLeftButtonUp += ScrollViewer_PreviewMouseLeftButtonUp;

            try
            {
                fileLabel.Text = Path.GetFileNameWithoutExtension(filePath);
                document = PdfDocument.Load(filePath);
                currentPage = 0;
                RenderPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Die PDF-Datei konnte nicht geladen werden:\n" + ex.Message,
                    "PDF-Reader", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RenderPage()
        {
            if (document == null || document.PageCount == 0)
            {
                return;
            }

            if (currentPage < 0) currentPage = 0;
            if (currentPage > document.PageCount - 1) currentPage = document.PageCount - 1;

            float dpi = (float)(BaseDpi * zoom);
            using (System.Drawing.Image img = document.Render(currentPage, dpi, dpi, false))
            {
                pageImage.Source = ToBitmapSource(img);
            }

            pageInfo.Text = (currentPage + 1) + " / " + document.PageCount;
            txtPage.Text = (currentPage + 1).ToString();
        }

        /// <summary>System.Drawing.Image (GDI+) in eine WPF-BitmapSource umwandeln.</summary>
        private static BitmapSource ToBitmapSource(System.Drawing.Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (document == null) return;
            if (currentPage > 0)
            {
                currentPage--;
                RenderPage();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (document == null) return;
            if (currentPage < document.PageCount - 1)
            {
                currentPage++;
                RenderPage();
            }
        }

        private void GotoButton_Click(object sender, RoutedEventArgs e)
        {
            GotoEnteredPage();
        }

        private void TxtPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GotoEnteredPage();
            }
        }

        private void GotoEnteredPage()
        {
            if (document == null) return;
            int pageNumber;
            if (int.TryParse(txtPage.Text, out pageNumber))
            {
                // Eingabe ist 1-basiert, intern 0-basiert.
                currentPage = pageNumber - 1;
                RenderPage();
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            // Zoom-Richtung gemaess Praxis-Feedback umgedreht.
            SetZoom(zoom / ZoomStep);
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            SetZoom(zoom * ZoomStep);
        }

        private void SetZoom(double newZoom)
        {
            if (newZoom < MinZoom) newZoom = MinZoom;
            if (newZoom > MaxZoom) newZoom = MaxZoom;
            if (Math.Abs(newZoom - zoom) < 0.0001) return;
            zoom = newZoom;
            RenderPage();
        }

        // ----- Drag-Panning -----
        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            panStartPoint = e.GetPosition(scrollViewer);
            panStartHOffset = scrollViewer.HorizontalOffset;
            panStartVOffset = scrollViewer.VerticalOffset;
            isPanning = true;
            scrollViewer.CaptureMouse();
            scrollViewer.Cursor = Cursors.ScrollAll;
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanning)
            {
                return;
            }
            System.Windows.Point p = e.GetPosition(scrollViewer);
            scrollViewer.ScrollToHorizontalOffset(panStartHOffset - (p.X - panStartPoint.X));
            scrollViewer.ScrollToVerticalOffset(panStartVOffset - (p.Y - panStartPoint.Y));
        }

        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isPanning)
            {
                return;
            }
            isPanning = false;
            scrollViewer.ReleaseMouseCapture();
            scrollViewer.Cursor = Cursors.Arrow;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (document != null)
            {
                document.Dispose();
                document = null;
            }
        }
    }
}
