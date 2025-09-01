using Microsoft.VisualBasic;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BW_to_WandAlpha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        const string titre = "Black and White → White and Alpha (Transparent)";
        Mat source;
        Dictionary<string, Mat> mats;
        Color color = Color.FromRgb(0, 0, 0);

        #region BINDING
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public string _folder_IN
        {
            get { return Properties.Settings.Default.folder_IN.ToString(); }
            set
            {
                Properties.Settings.Default.folder_IN = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(_folder_IN));
            }
        }

        public string _folder_OUT
        {
            get { return Properties.Settings.Default.folder_OUT.ToString(); }
            set
            {
                Properties.Settings.Default.folder_OUT = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(_folder_OUT));
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _colorPickerJJO._ColorNew += ColorNew;
            //_colorPickerJJO._SetMouseSelection(0.5, 0.5);
            _colorPickerJJO._SetColor(Colors.Magenta);
        }

        #region UI
        private void btn_ReadDirectory_click(object sender, MouseButtonEventArgs e)
        {
            mats = new Dictionary<string, Mat>();

            if (!System.IO.Directory.Exists(_folder_IN))
                return;
            string[] fichiers = System.IO.Directory.GetFiles(_folder_IN);
            lb.Items.Clear();

            bool first = true;
            foreach (string fichier in fichiers)
            {
                Mat mat = new Mat(fichier, ImreadModes.Unchanged);
                if (mat.Empty())
                    continue;
                source = mat;
                mats.Add(fichier, mat);
                lb.Items.Add(newItem(mat, fichier));
                if (first)
                {
                    first = false;
                    lb.SelectedIndex = 0;
                }
                //Application.Current.
                //Dispatcher.BeginInvoke(new Action(() =>                    {
                Title = titre + " - " + lb.Items.Count + " / " + fichiers.Length;
                //}));
                System.Threading.Thread.Sleep(1);
            }


        }

        void btn_CreateNewDirectory_click(object sender, MouseButtonEventArgs e)
        {
            string tmp = _folder_IN + "\\new";
            if (System.IO.Directory.Exists(tmp))
                if (MessageBox.Show(tmp + "\nalready exists. Continue ?", "Already exists", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            _folder_OUT = tmp;
        }

        void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessOnSelectedItem();
        }

        private void ckb_management(object sender, RoutedEventArgs e)
        {
            if (source != null)
                ProcessOnSelectedItem();
        }

        void btn_SaveSelected_click(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)lb.SelectedItem;
            if (item == null)
                return;
            string msg = Process(item);
            MessageBox.Show(msg);

        }

        string Process(ListBoxItem item)
        {
            string msg;
            try
            {
                string path = item.ToolTip.ToString();
                Mat mat = mats[path];
                Mat mat_out = ImageProcessing(mat);

                //create folder if missing
                System.IO.Directory.CreateDirectory(_folder_OUT);

                string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                string fullfilename = _folder_OUT + "\\" + filename + ".png";
                mat_out.SaveImage(fullfilename);
                msg = fullfilename + " created";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                MessageBox.Show(msg);
            }
            return msg;
        }

        void btn_SaveAll_click(object sender, MouseButtonEventArgs e)
        {
            List<string> msgs = new List<string>();
            foreach (ListBoxItem item in lb.Items)
                msgs.Add(Process(item));

            MessageBox.Show(string.Join(",\n", msgs));
        }

        #endregion

        ListBoxItem newItem(Mat mat, string fichier)
        {
            ListBoxItem item = new ListBoxItem();
            Image img = new Image();
            img.Source = mat.ToBitmapSource();// conversion.ToImageSource(mat);
            img.Height = 100;
            img.Stretch = Stretch.Uniform;
            item.Content = img;
            item.ToolTip = fichier;
            return item;
        }

        Mat ImageProcessing(Mat src)
        {
            Mat gray = new Mat();

            Mat newmat = new Mat();
            Mat gray_not;
            Mat[] bgra;
            //cas  où on a une image transparente
            switch (src.Channels())
            {
                case 0:
                    return newmat;

                case 1:
                    return newmat;

                case 2:
                    return newmat;

                case 3:
                    Cv2.CvtColor(src, gray, ColorConversionCodes.RGB2GRAY);

                    gray_not = new Mat();
                    if (ckb_not_a.IsChecked == true)
                        Cv2.BitwiseNot(gray, gray_not);

                    bgra = new Mat[] { Mat.Ones(src.Size(),MatType.CV_8UC1) * color.B,
                                     Mat.Ones(src.Size(),MatType.CV_8UC1) * color.G,
                                     Mat.Ones(src.Size(),MatType.CV_8UC1) * color.R,
                                     (ckb_not_a.IsChecked==true)? gray_not:gray};

                    Cv2.Merge(bgra, newmat);
                    return newmat;

                case 4:
                    Mat[] channels = src.Split();
                    Mat src3channels = new Mat();

                    Cv2.Merge(new Mat[] { channels[0], channels[1], channels[2] }, src3channels);

                    Cv2.CvtColor(src3channels, gray, ColorConversionCodes.RGB2GRAY);

                    gray_not = new Mat();
                    if (ckb_not_a.IsChecked == true)
                        Cv2.BitwiseNot(gray, gray_not);

                    bgra = new Mat[] { Mat.Ones(src.Size(),MatType.CV_8UC1) * color.B,
                                     Mat.Ones(src.Size(),MatType.CV_8UC1) * color.G,
                                     Mat.Ones(src.Size(),MatType.CV_8UC1) * color.R,
                                     channels[3]};

                    Cv2.Merge(bgra, newmat);
                    return newmat;
            }
            return newmat;
        }

        void ProcessOnSelectedItem()
        {
            if (lb.SelectedItem == null) return;
            ListBoxItem item = (ListBoxItem)lb.SelectedItem;
            Mat mat = mats[item.ToolTip.ToString()];
            img_before.Source = mat.ToBitmapSource();// conversion.ToImageSource(mat);
            Mat mat_out = ImageProcessing(mat);
            img_after.Source = mat_out.ToBitmapSource();// conversion.ToImageSource(mat_out);
        }

        void ColorNew(object sender, ColorPickerJJO.NewColorEventArgs e)
        {
            color = e.color;
            _colorRGB.Content = "RGB (" + e.color.R + ", " + e.color.G + ", " + e.color.B + ")";
            _ColorPicker.Background = new SolidColorBrush(e.color);
            ProcessOnSelectedItem();
        }
    }
}