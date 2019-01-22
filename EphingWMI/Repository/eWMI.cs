using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace EphingWMI.Repository
{
    public class eWMI
    {
        public List<ManagementObject> InvokeQuery(string computerName, string Query, string Namespace)
        {
            List<ManagementObject> returnList = new List<ManagementObject>();
            if (String.IsNullOrEmpty(computerName))
            {
                computerName = "localhost";
            }
            string fullNameSpace = @"\\" + computerName + @"\" + Namespace;
            ManagementScope scope = new ManagementScope(fullNameSpace);
            scope.Connect();
            ObjectQuery objQuery = new ObjectQuery(Query);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, objQuery))
            {
                foreach(var obj in searcher.Get().OfType<ManagementObject>())
                {
                    returnList.Add(obj);
                }
            }
            return returnList;
        }

        public List<string> GetNamespaceClasses(string computerName, string Namespace)
        {
            var returnList = new List<string>();
            string query = "Select * From meta_class";
            var managementCol = InvokeQuery(computerName, query, Namespace);
            foreach(var managementObject in managementCol)
            {
                string field = managementObject.ToString();
                string[] parts = field.Split(new char[] { ':' });
                returnList.Add(parts[1]);
            }
            return returnList;
        }
    }
}
