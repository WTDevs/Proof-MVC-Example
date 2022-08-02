using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proof.Core.ViewModels
{
    public class SelectedFilterValues
    {
        public string Event { get; set; }
        public string AssignedTo { get; set; }
        public string FMA { get; set; }
        public string Status { get; set; }
        public string CustomerImpact { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
