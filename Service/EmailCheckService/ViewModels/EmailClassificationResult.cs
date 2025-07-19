using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.EmailCheckService.ViewModels
{
    public class EmailClassificationResult
    {
        public string Category { get; set; }
        public double Confidence { get; set; }
        public List<string> Indicators { get; set; } = new();
        public string Level { get; set; }
    }
}
