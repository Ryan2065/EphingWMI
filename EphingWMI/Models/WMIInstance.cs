using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EphingWMI.Models
{
    public class WMIInstance
    {
        public List<WMIInstanceProperty> Properties { get; set; }
        public List<WMIMethod> Methods { get; set; }
        public WMIInstance()
        {
            Properties = new List<WMIInstanceProperty>();
            Methods = new List<WMIMethod>();
        }
    }
    public class WMIInstanceProperty
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        
    }
    public class WMIMethod
    {
        public string Name { get; set; }
        public List<Parameter> InputParameters { get; set; }
        public string OutputType { get; set; }
        public WMIMethod()
        {
            InputParameters = new List<Parameter>();
        }
    }
    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
