using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using EphingWMI.Repository;
using EphingWMI.Models;

namespace EphingWMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConnectButtonBackgroundWorker connectButtonBackgroundWorker;
        private NamespaceSelectionChangedBackgroundWorker namespaceSelectionChanged;
        private ClassSelectionChangedBackgroundWorker classSelectionChangedBackgroundWorker;
        public static MainWindow AppWindow;
        public ObservableCollection<WMIInstanceProperty> WMIInstanceList;
        public bool Grid_Instances_IsEnabled;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
            connectButtonBackgroundWorker = new ConnectButtonBackgroundWorker();
            namespaceSelectionChanged = new NamespaceSelectionChangedBackgroundWorker();
            classSelectionChangedBackgroundWorker = new ClassSelectionChangedBackgroundWorker();
            WMIInstanceList = new ObservableCollection<WMIInstanceProperty>();
            Grid_Instances_IsEnabled = true;
            Progress_Instances.Visibility = Visibility.Hidden;
            TextBlock_Instances.Visibility = Visibility.Hidden;
        }

        private void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            connectButtonBackgroundWorker.RunWorker(TxtComputerName.Text);
        }

        private void Namespace_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selItem = TreeviewNamespaces.SelectedItem as TreeViewItem;
            namespaceSelectionChanged.RunWorker(TxtComputerName.Text, selItem.Tag.ToString());
        }

        private void ClassFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyText = TxtFilterClasses.Text;
            foreach(ListViewItem item in ListClasses.Items)
            {
                
                if (String.IsNullOrEmpty(keyText))
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    string content = item.Content.ToString();
                    if (content.ToLower().Contains(keyText.ToLower()))
                    {
                        item.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void ListClasses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ListClasses.SelectedItem as ListViewItem;
            if(selectedItem != null)
            {
                TextClassQuery.Text = "Select * from " + selectedItem.Content.ToString();
            }
        }

        private void RunQuery_OnClick(object sender, RoutedEventArgs e)
        {
            var selItem = TreeviewNamespaces.SelectedItem as TreeViewItem;
            classSelectionChangedBackgroundWorker.RunWorker(TxtComputerName.Text, selItem.Tag.ToString(), TextClassQuery.Text);
        }
        
    }
}
