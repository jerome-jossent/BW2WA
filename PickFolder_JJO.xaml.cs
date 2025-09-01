using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BW_to_WandAlpha
{
    public partial class PickFolder_JJO : UserControl
    {
        public bool _ModeTBX
        {
            get => ModeTBX; set
            {
                ModeTBX = value;
                if (ModeTBX)
                {
                    _modelbl = Visibility.Collapsed;
                    _modetbx = Visibility.Visible;
                }
                else
                {
                    _modelbl = Visibility.Visible;
                    _modetbx = Visibility.Collapsed;
                }
            }
        }
        bool ModeTBX;

        public Visibility _modelbl { get; set; }
        public Visibility _modetbx { get; set; }

        public PickFolder_JJO()
        {
            InitializeComponent();
            DataContext = this; //2025/08/25 ??
        }

        public string _folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        public static readonly DependencyProperty FolderProperty =
                                    DependencyProperty.Register("_folder",
                                        typeof(string),
                                        typeof(PickFolder_JJO),
                                        new FrameworkPropertyMetadata(null, OnTextBindingChanged));
        private static void OnTextBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
            PickFolder_JJO pf = d as PickFolder_JJO;
            pf._folder = e.NewValue as string;
            }
        }

        private void Folder_click(object sender, MouseButtonEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //default folder
            if (System.IO.Directory.Exists(_folder))
                dialog.SelectedPath = _folder;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                _folder = dialog.SelectedPath;
        }
    }
}