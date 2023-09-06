using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace SearchInExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataTable GridData;
        public string explorePath = String.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGridData();
        }

        public void InitializeGridData()
        {
            GridData = new DataTable();
            GridData.Columns.Add(new DataColumn("Name", typeof(string)));
            GridData.Columns.Add(new DataColumn("Attribute", typeof(string)));

            var t = Type.GetTypeFromProgID("Shell.Application");
            dynamic o = Activator.CreateInstance(t);
            try
            {
                var ws = o.Windows();
                for (int i = 0; i < ws.Count; i++)
                {
                    var ie = ws.Item(i);
                    if (ie == null) continue;
                    var path = System.IO.Path.GetFileName((string)ie.FullName);
                    if (path.ToLower() == "explorer.exe")
                    {
                        explorePath = System.IO.Path.GetDirectoryName(ie.document.focuseditem.path);
                    }
                }
                if (explorePath != null && explorePath != string.Empty)
                {
                    GridData.Clear();

                    DirectoryInfo d = new DirectoryInfo(explorePath); //Assuming Test is your Folder

                    DirectoryInfo[] Directories = d.GetDirectories(); // Getting directory names

                    FileInfo[] Files = d.GetFiles(); //Getting files

                    foreach (DirectoryInfo dir in Directories)
                    {
                        GridData.Rows.Add(dir.Name, dir.Attributes.ToString().ToUpper());
                    }

                    foreach (FileInfo file in Files)
                    {
                        GridData.Rows.Add(file.Name, file.Attributes.ToString().ToUpper() );
                        
                    }
                    datagrid.ItemsSource = GridData.DefaultView;
                }
                else
                {
                    MessageBox.Show("No instance of FileExplorer is currently open...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(o);

            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox.Text != String.Empty)
            {
                var searchTerm = txtBox.Text.Trim();
                GridData.DefaultView.RowFilter = $"Name LIKE '%{searchTerm}%'";
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeGridData();
        }
    }
}
