using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Standard_UC_JJO
{
    /// <summary>
    /// Logique d'interaction pour PickFile_JJO.xaml
    /// </summary>
    public partial class PickFile_JJO : UserControl
    {
        public PickFile_JJO()
        {
            InitializeComponent();
        }

        public string _filepath
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }
        public static readonly DependencyProperty FolderProperty =
                                    DependencyProperty.Register("_filepath",
                                        typeof(string),
                                        typeof(PickFile_JJO),
                                        new FrameworkPropertyMetadata(null, OnTextBindingChanged));

        /// <summary>
        /// "txt files (*.txt)|*.txt|All files (*.*)|*.*";
        /// </summary>
        public string _fileextensions
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }
        public static readonly DependencyProperty FileProperty =
                                    DependencyProperty.Register("_fileextensions",
                                        typeof(string),
                                        typeof(PickFile_JJO),
                                        new FrameworkPropertyMetadata("All files(*.*) | *.*", OnTextBindingChanged2));

        private static void OnTextBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PickFile_JJO pf = d as PickFile_JJO;
            pf._filepath = e.NewValue as string;
        }
        private static void OnTextBindingChanged2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void File_click(object sender, MouseButtonEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            //default filepath
            if (System.IO.File.Exists(_filepath))
                dialog.FileName = _filepath;
            dialog.Filter = _fileextensions;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                _filepath = dialog.FileName;
        }
    }
}
