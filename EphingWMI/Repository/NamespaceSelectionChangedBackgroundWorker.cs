using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EphingWMI.Repository
{
    public class NamespaceSelectionChangedBackgroundWorker
    {
        private BackgroundWorker backgroundWorker;
        bool _restart = false;
        string _computerName = "";
        string _namespace = "";

        public NamespaceSelectionChangedBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorkerBackgroundJob);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerBackgroundJobComplete);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerBackgroundJobProgress);
        }

        public void RunWorker(string computerName, string nameSpace)
        {
            if (backgroundWorker.IsBusy)
            {
                _restart = true;
                _computerName = computerName;
                _namespace = nameSpace;
                backgroundWorker.CancelAsync();
                return;
            }
            MainWindow.AppWindow.ListClasses.Items.Clear();
            string[] workerArgs = new string[2]; //(computerName, nameSpace);
            workerArgs[0] = computerName;
            workerArgs[1] = nameSpace;
            backgroundWorker.RunWorkerAsync(workerArgs);
        }

        private void backgroundWorkerBackgroundJob(object sender, DoWorkEventArgs e)
        {
            var wmiHelpers = new eWMI();
            string computerName = "";
            string nameSpace = "";
            string[] args;
            try
            {
                args = e.Argument as string[];
                computerName = args[0];
                nameSpace = args[1];
            }
            catch
            {
                throw;
            }
            var classList = wmiHelpers.GetNamespaceClasses(computerName, nameSpace);
            backgroundWorker.ReportProgress(1, classList);
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorkerBackgroundJobComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_restart)
            {
                RunWorker(_computerName, _namespace);
                _restart = false;
                _computerName = "";
                _namespace = "";
            }
        }

        private void backgroundWorkerBackgroundJobProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null && !backgroundWorker.CancellationPending)
            {
                MainWindow.AppWindow.ListClasses.Items.Clear();
                List<string> classList = e.UserState as List<string>;
                foreach(string classObject in classList)
                {
                    var listViewItem = new ListViewItem();
                    listViewItem.Content = classObject;
                    MainWindow.AppWindow.ListClasses.Items.Add(listViewItem);
                }
                MainWindow.AppWindow.ListClasses.Items.SortDescriptions.Clear();
                MainWindow.AppWindow.ListClasses.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
            }
        }
    }
}
