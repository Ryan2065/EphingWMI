using EphingWMI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphingWMI.Repository
{
    public class ClassSelectionChangedBackgroundWorker
    {
        private BackgroundWorker backgroundWorker;
        bool _restart = false;
        string _computerName = "";
        string _namespace = "";
        string _query = "";
        public ClassSelectionChangedBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorkerBackgroundJob);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerBackgroundJobComplete);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerBackgroundJobProgress);
        }

        public void RunWorker(string computerName, string nameSpace, string Query)
        {
            if (backgroundWorker.IsBusy)
            {
                _restart = true;
                _computerName = computerName;
                _namespace = nameSpace;
                _query = Query;
                backgroundWorker.CancelAsync();
                return;
            }
            MainWindow.AppWindow.ListClasses.Items.Clear();
            string[] workerArgs = new string[3];
            workerArgs[0] = computerName;
            workerArgs[1] = nameSpace;
            workerArgs[2] = Query;
            backgroundWorker.RunWorkerAsync(workerArgs);
        }

        private void backgroundWorkerBackgroundJobProgress(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        private void backgroundWorkerBackgroundJobComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_restart)
            {
                RunWorker(_computerName, _namespace, _query);
                _restart = false;
                _computerName = "";
                _namespace = "";
                _query = "";
            }
        }

        private void backgroundWorkerBackgroundJob(object sender, DoWorkEventArgs e)
        {
            var wmiHelpers = new eWMI();
            string computerName = "";
            string nameSpace = "";
            string wmiQuery = "";
            string[] args;
            try
            {
                args = e.Argument as string[];
                computerName = args[0];
                nameSpace = args[1];
                wmiQuery = args[2];
                var results = wmiHelpers.InvokeQuery(computerName, wmiQuery, nameSpace);
                foreach(var result in results)
                {
                    
                    WMIInstance tempInstance = new WMIInstance();
                    foreach(var sysProperty in result.SystemProperties)
                    {
                        tempInstance.Properties.Add(new WMIInstanceProperty
                        {
                            PropertyName = sysProperty.Name,
                            Type = sysProperty.Type.ToString(),
                            Value = sysProperty.Value
                        });
                    }
                    foreach(var Property in result.Properties)
                    {
                        tempInstance.Properties.Add(new WMIInstanceProperty
                        {
                            PropertyName = Property.Name,
                            Type = Property.Type.ToString(),
                            Value = Property.Value
                        });
                    }
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
