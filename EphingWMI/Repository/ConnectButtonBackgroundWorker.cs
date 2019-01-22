using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EphingWMI.Repository
{
    public class ConnectButtonBackgroundWorker
    {
        private BackgroundWorker backgroundWorker;
        bool _restart = false;
        string _computerName = "";
        public ConnectButtonBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorkerBackgroundJob);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerBackgroundJobComplete);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerBackgroundJobProgress);
        }

        public void RunWorker(string computerName)
        {
            if(backgroundWorker.IsBusy)
            {
                _restart = true;
                _computerName = computerName;
                backgroundWorker.CancelAsync();
                return;
            }
            MainWindow.AppWindow.TreeviewNamespaces.Items.Clear();
            MainWindow.AppWindow.TreeviewNamespaces.Items.Add(new TreeViewItem
            {
                Header = "root",
                Tag = "root",
                IsExpanded = true
            });
            backgroundWorker.RunWorkerAsync(computerName);
        }
        
        private void backgroundWorkerBackgroundJob(object sender, DoWorkEventArgs e)
        {
            string computerName = "";
            if(e.Argument != null)
            {
                computerName = e.Argument.ToString();
            }
            WMIRecursiveSearch("root", computerName);
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void WMIRecursiveSearch(string nameSpace, string computerName)
        {
            var wmiHelpers = new eWMI();
            if (backgroundWorker.CancellationPending)
            {
                return;
            }
            try
            {
                var results = wmiHelpers.InvokeQuery(computerName, "Select Name from __Namespace", nameSpace);
                foreach (var result in results)
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        return;
                    }
                    string childNamespaceName = nameSpace + "\\" + result.GetPropertyValue("Name").ToString();
                    backgroundWorker.ReportProgress(1, childNamespaceName);
                    WMIRecursiveSearch(childNamespaceName, computerName);
                }
            }
            catch
            {
                backgroundWorker.ReportProgress(1, nameSpace + "\\" + "Error getting child namespaces");
            }
        }

        private void backgroundWorkerBackgroundJobComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_restart)
            {
                RunWorker(_computerName);
                _restart = false;
                _computerName = "";
            }
        }

        private void backgroundWorkerBackgroundJobProgress(object sender, ProgressChangedEventArgs e)
        {
            if(e.UserState != null && !backgroundWorker.CancellationPending)
            {
                string[] nameSpaces = e.UserState.ToString().Split('\\');
                var treeViewCollection = MainWindow.AppWindow.TreeviewNamespaces.Items;
                string currentNamespacePath = "";
                for (int i = 0; i < nameSpaces.Count(); i++)
                {
                    string parentNamespace = nameSpaces[i];
                    if(String.IsNullOrEmpty(currentNamespacePath))
                    {
                        currentNamespacePath = parentNamespace;
                    }
                    else
                    {
                        currentNamespacePath = currentNamespacePath + @"\" + parentNamespace;
                    }
                    bool foundParent = false;
                    foreach(TreeViewItem item in treeViewCollection)
                    {
                        if(item.Header.ToString() == parentNamespace)
                        {
                            treeViewCollection = item.Items;
                            foundParent = true;
                            break;
                        }
                    }
                    if(foundParent == false)
                    {
                        var newTreeViewItem = new TreeViewItem
                        {
                            Header = parentNamespace,
                            Tag = currentNamespacePath
                        };
                        treeViewCollection.Add(newTreeViewItem);
                        treeViewCollection.SortDescriptions.Clear();
                        treeViewCollection.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
                        treeViewCollection = newTreeViewItem.Items;
                    }
                }
            }
        }
    }
}
