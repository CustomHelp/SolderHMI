using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ExcelDataReader;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpExcelReader.xaml
    /// Zeigt eine Excel-Datei (.xls / .xlsx) read-only als Tabelle an.
    /// Liest beide Formate über ExcelDataReader (automatische Formaterkennung).
    /// </summary>
    public partial class WindowPopUpExcelReader : Window
    {
        private DataSet dataSet;

        public WindowPopUpExcelReader(string filePath)
        {
            InitializeComponent();

            try
            {
                fileLabel.Text = Path.GetFileNameWithoutExtension(filePath);

                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Erste Zeile als Spaltenkopf verwenden (schlichte, flache Tabellen).
                    var config = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    dataSet = reader.AsDataSet(config);
                }

                LoadSheets();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Die Excel-Datei konnte nicht geladen werden:\n" + ex.Message,
                    "Excel-Reader", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadSheets()
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return;
            }

            sheetCB.Items.Clear();
            foreach (DataTable table in dataSet.Tables)
            {
                sheetCB.Items.Add(table.TableName);
            }

            // Blattauswahl nur einblenden, wenn mehr als ein Tabellenblatt vorhanden ist.
            bool multipleSheets = dataSet.Tables.Count > 1;
            sheetCB.Visibility = multipleSheets ? Visibility.Visible : Visibility.Collapsed;
            sheetCaption.Visibility = multipleSheets ? Visibility.Visible : Visibility.Collapsed;

            sheetCB.SelectedIndex = 0; // löst SheetCB_SelectionChanged aus -> zeigt erstes Blatt
        }

        private void SheetCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataSet == null || sheetCB.SelectedIndex < 0 || sheetCB.SelectedIndex >= dataSet.Tables.Count)
            {
                return;
            }

            dataGrid.ItemsSource = dataSet.Tables[sheetCB.SelectedIndex].DefaultView;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
