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
using System.Windows.Controls.Primitives;
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.UserControls.LOET;
using LOET_HMI.UserControls;


namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgKROSY.xaml
    /// </summary>
    public partial class PgKROSY : Page, INotifyPropertyChanged
    {
        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpKROSY = DependencyProperty.Register(
            "DpKROSY", typeof(StKROSY), typeof(PgKROSY), new PropertyMetadata()); // Hinweis, warum die PropertyMetadata null ist: https://stackoverflow.com/questions/20946705/why-do-different-instances-have-same-dependency-property-value

        public StKROSY DpKROSY
        {
            get { return (StKROSY)GetValue(_DpKROSY); }
            set { SetValue(_DpKROSY, value); }
        }

        // *******************************************************************************************
        // *******************************************************************************************
        // *******************************************************************************************
        public List<Binding> listBindingsStat { get; set; }
        public List<Binding> listBindingsStatObj { get; set; }

        public List<TextBox> listAllTextBox { get; set; }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PgKROSY()
        {
            DpKROSY = new StKROSY();

            InitializeComponent();

            listAllTextBox = new List<TextBox>();
            listAllTextBox.Add(tbHostname);
            listAllTextBox.Add(tbIP);
            listAllTextBox.Add(tbVersion);
            listAllTextBox.Add(tbLanguage);
            listAllTextBox.Add(tbDeviceName);
            listAllTextBox.Add(tbOrderMax);
            listAllTextBox.Add(tbObjectMax);



            listBindingsStat = new List<Binding>();
            listBindingsStatObj = new List<Binding>();

            ucStation1.iActObject = 1;
            ucStation2.iActObject = 1;

            ucStation1.btnLeft.Click += BtnLeft_Click;
            ucStation2.btnLeft.Click += BtnLeft_Click;
            ucStation1.btnRight.Click += BtnRight_Click;
            ucStation2.btnRight.Click += BtnRight_Click;
            ucStation1.btnReset.Click += BtnReset_Click;
            ucStation2.btnReset.Click += BtnReset_Click;
        }

        private void PgKROSY_Loaded(object sender, RoutedEventArgs e)
        {
            LOET_RFID_Functions.FormatTextBox_READ(listAllTextBox);

            DpKROSY.Register();

            SetBindingsOfStations(_ucStation: ucStation1);
            SetBindingsOfStations(_ucStation: ucStation2);
            SetBindingsOfStationObjects(_ucStation: ucStation1, _iObjectNr: ucStation1.iActObject);
            SetBindingsOfStationObjects(_ucStation: ucStation2, _iObjectNr: ucStation2.iActObject);
        }

        private void PgKROSY_Unloaded(object sender, RoutedEventArgs e)
        {
            DpKROSY.Deregister();
        }


        private void SetBindingsOfStations(UcStation _ucStation)
        {
            int iStationNr = _ucStation.UCStationNr;
            string strPathStStation = nameof(DpKROSY) + "." + nameof(DpKROSY.arrStation) + ".[" + (iStationNr - 1).ToString() + "].";
            var tempStStation = new StKROSY.StStation(-1); // erstellt nur, damit auf die Strukturkomponenten zugegriffern werden kann und diese in String umgewandelt werdenkönnen

            listBindingsStat = new List<Binding>();
            listBindingsStat.Add(new Binding(strPathStStation + nameof(tempStStation.strscanCode)));
            listBindingsStat.Add(new Binding(strPathStStation + nameof(tempStStation.strKSIdent_ident)));
            listBindingsStat.Add(new Binding(strPathStStation + nameof(tempStStation.iAmount)));
            listBindingsStat.Add(new Binding(strPathStStation + nameof(tempStStation.iAmountOK)));

            for(int k=0; k<listBindingsStat.Count; k++)
            {
                listBindingsStat[k].Source = pgKROSY;
                listBindingsStat[k].UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }

            try
            {
                _ucStation.tbScancode.SetBinding(TextBox.TextProperty, listBindingsStat.Single(b => b.Path.Path == strPathStStation + nameof(tempStStation.strscanCode)));
                _ucStation.tbKSIdent.SetBinding(TextBox.TextProperty, listBindingsStat.Single(b => b.Path.Path == strPathStStation + nameof(tempStStation.strKSIdent_ident)));
                _ucStation.tbAmount.SetBinding(TextBox.TextProperty, listBindingsStat.Single(b => b.Path.Path == strPathStStation + nameof(tempStStation.iAmount)));
                _ucStation.tbAmountOK.SetBinding(TextBox.TextProperty, listBindingsStat.Single(b => b.Path.Path == strPathStStation + nameof(tempStStation.iAmountOK)));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erstellung der Bindings in " + nameof(pgKROSY) + " fehlgeschlagen\n\n" + ex.Message,
                            nameof(pgKROSY) + " " + nameof(SetBindingsOfStations),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            }

        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            Button btnClicked = (Button)sender;
            UcStation ucStationClicked = UIHelper.TryFindParent<UcStation>(btnClicked);

            try
            {
                if (ucStationClicked.iActObject > 1)
                {
                    ucStationClicked.iActObject = ucStationClicked.iActObject - 1;
                    SetBindingsOfStationObjects(_ucStation: ucStationClicked, _iObjectNr: ucStationClicked.iActObject); // ObjectNr um 1 reduzieren und die Bindings anpassen
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                            nameof(pgKROSY) + " " + nameof(BtnLeft_Click),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                
            }
            
        }


        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            Button btnClicked = (Button)sender;
            UcStation ucStationClicked = UIHelper.TryFindParent<UcStation>(btnClicked);

            try
            {
                if (ucStationClicked.iActObject < DpKROSY.stGlobal.iObjectMax)
                {
                    ucStationClicked.iActObject = ucStationClicked.iActObject + 1;
                    SetBindingsOfStationObjects(_ucStation: ucStationClicked, _iObjectNr: ucStationClicked.iActObject);// ObjectNr um 1 erhöhen und die Bindings anpassen
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.Message,
                            nameof(pgKROSY) + " " + nameof(BtnRight_Click),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            }

        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Button btnClicked = (Button)sender;
            UcStation ucStationClicked = UIHelper.TryFindParent<UcStation>(btnClicked);

            try
            {
                DpKROSY.ResetStation(ucStationClicked.UCStationNr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                           nameof(pgKROSY) + " " + nameof(BtnReset_Click),
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
            }
        }


        private void SetBindingsOfStationObjects(UcStation _ucStation, int _iObjectNr)
        {
            int iStationNr = _ucStation.UCStationNr;
            int iObjNr = _iObjectNr;
            string strPathStStatObj = nameof(DpKROSY) + "." + nameof(DpKROSY.arrStation) + ".[" + (iStationNr - 1).ToString() + "].arrStationObjects[" + (iObjNr - 1).ToString() + "].";
            var tempStStatObj = new StKROSY.StStationObject(-1); // erstellt nur, damit auf die Strukturkomponenten zugegriffern werden kann und diese in String umgewandelt werdenkönnen

            listBindingsStatObj = new List<Binding>();

            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.iKROSYstate_objecttype)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.iKROSYstate_objectstate)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.iPLCstate_objecttype)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.iPLCstate_objectstate)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.strKSIdent_ident)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.i_WTNr)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.i_INDNr)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.iOverride)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.b_DVS1_ON)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.b_DVS2_ON)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.b_DVS3_ON)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.bIO)));
            listBindingsStatObj.Add(new Binding(strPathStStatObj + nameof(tempStStatObj.bNIO)));


            for (int k = 0; k < listBindingsStatObj.Count; k++)
            {
                listBindingsStatObj[k].Source = pgKROSY;
                listBindingsStatObj[k].UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }

            try
            {
                _ucStation.tbKROSYState_ObjType.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.iKROSYstate_objecttype)));
                _ucStation.tbKROSYState_ObjState.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.iKROSYstate_objectstate)));
                _ucStation.tbPLCState_ObjType.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.iPLCstate_objecttype)));
                _ucStation.tbPLCState_ObjState.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.iPLCstate_objectstate)));
                _ucStation.tbksidnetObject.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.strKSIdent_ident)));
                _ucStation.tbWTNr.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.i_WTNr)));
                _ucStation.tbINDNr.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.i_INDNr)));
                _ucStation.tbOverride.SetBinding(TextBox.TextProperty, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.iOverride)));

                _ucStation.lampDVS1On.SetBinding(UcLamp._IsON, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.b_DVS1_ON)));
                _ucStation.lampDVS2On.SetBinding(UcLamp._IsON, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.b_DVS2_ON)));
                _ucStation.lampDVS3On.SetBinding(UcLamp._IsON, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.b_DVS3_ON)));
                _ucStation.lampIO.SetBinding(UcLamp._IsON, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.bIO)));
                _ucStation.lampNIO.SetBinding(UcLamp._IsON, listBindingsStatObj.Single(b => b.Path.Path == strPathStStatObj + nameof(tempStStatObj.bNIO)));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erstellung der Bindings in " + nameof(pgKROSY) + " fehlgeschlagen\n\n" + ex.Message,
                            nameof(pgKROSY) + " " + nameof(SetBindingsOfStationObjects),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            }
        }
    }



    #region Find Parent
    /// <summary>
    /// MBA 14.05.2021, Quelle: http://www.hardcodet.net/2008/02/find-wpf-parent
    /// </summary>
    public static class UIHelper
    {

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null) return null;

            //handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
    }


    #endregion

}
