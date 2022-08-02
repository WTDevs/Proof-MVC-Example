using System.Collections.Generic;

namespace Proof.Core.ViewModels
{
    public class IncidentsVM
    {
        public List<Proof.Core.Incident> IncidentsList { get; set; }
        public IncidentsFilterOptions FilterOptions { get; set; }

        public IncidentsVM()
        {
            IncidentsList= new List<Proof.Core.Incident>();
            FilterOptions = new IncidentsFilterOptions();
        }

    }
}
