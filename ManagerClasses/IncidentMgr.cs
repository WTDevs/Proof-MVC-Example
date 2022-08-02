using Proof.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Proof.Core.ManagerClasses;

namespace Proof.Core.ManagerClasses
{
    public class IncidentMgr
    {
        private readonly ProofEntities db = new ProofEntities();

        LocationMgr _LocationMgr = new LocationMgr();
        RegionMgr _RegionsMgr = new RegionMgr();

        public IncidentMgr() { }

        public IEnumerable<Incident> GetIncidentsByEventId(int eventId)
        {
            var incidents = db.Incidents.Where(r => r.IsDeleted == null).Include("XrCondition").Include("XrPriority").Include("XrStatu").Include("XrEvent").ToList();

            if (eventId > 0)
            {
                incidents = incidents.Where(x => x.EventId == eventId).ToList();
            }

            return incidents;
        }
        public IEnumerable<Incident> GetIncidentsFiltered(string ddlEvent, string ddlAssignedTo, string ddlFMA, string ddlStatus, string ddlCustomerImpact, DateTime? dtStartDate, DateTime? dtEndDate)
        {
            int ddlEventId = 0;
            int.TryParse(ddlEvent, out ddlEventId);

            var incidents = db.Incidents.Where(r =>
            (r.EventId == ddlEventId || ddlEventId == 0) &&
            (r.AssignTo == ddlAssignedTo || ddlAssignedTo == "") &&
            (r.FMA == ddlFMA || ddlFMA == "") &&
            (r.XrStatu.Status == ddlStatus || ddlStatus == "") &&
            (r.CustomerImpact == ddlCustomerImpact || ddlCustomerImpact == "") &&
            (r.XrEvent.StartDate >= dtStartDate || dtStartDate == null) &&
            (r.XrEvent.EndDate <= dtEndDate || dtEndDate == null)
            ).Include("XrCondition").Include("XrPriority").Include("XrStatu").Include("XrEvent").ToList();

            return incidents;
        }

        public IEnumerable<Incident> GetIncidentsWithAssociatedTablesByDateSelections(DateTime startDate, DateTime endDate)
        {
            return db.Incidents.Where(r => r.CreatedOn >= startDate && r.CreatedOn <= endDate && r.IsDeleted != true).Include("XrCondition").Include("XrPriority").Include("XrStatu").Include("XrEvent");
        }

        public Incident GetIncident(int id)
        {
            return db.Incidents.Find(id);
        }

        public void Update(Incident incident)
        {
            db.Entry(incident).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Create(Incident incident)
        {
            db.Entry(incident).State = EntityState.Added;
            db.SaveChanges();
        }

        public IEnumerable<Incident> GetIncidences(DateTime startDate, DateTime endDate, int eventId, int regionId)
        {
            // Date Range
            var queryResult = db.Incidents.Include("XrCondition").Where(x => x.CreatedOn >= startDate && x.CreatedOn <= endDate);

            // EventId
            if (eventId != 0) queryResult = queryResult.Where(x => x.EventId == eventId);

            // RegionId 
            if (regionId != 0)
            {
                queryResult = queryResult.Where(x => x.XrEvent.RegionId == regionId);
            }


            return queryResult;
        }

        public Incident GetIncidentWithCondition(int incidentID, int conditionsId)
        {
            return db.Incidents.Where(x => x.IncidentID == incidentID).Include("XrCondition").Where(x => x.ConditionsId == conditionsId).FirstOrDefault();
        }

        public IncidentsFilterOptions SetFilterDropDowns(int eventId)
        {
            string all = "";
            return SetFilterDropDowns(eventId, all, all, all, all, all, all);
        }
        public IncidentsFilterOptions SetFilterDropDowns(int eventId, string assignedTo, string fma, string status, string customerImpact, string startDate, string endDate)
        {
            var filterOptions = new IncidentsFilterOptions();
            filterOptions.StartDate = startDate;
            filterOptions.EndDate = endDate;

            if (eventId == 0)
            {
                filterOptions.EventOptions = new SelectList(
                        db.Incidents.Where(x => x.IsDeleted != true && x.XrEvent != null)
                        .Select(c => new ListValues() { ItemText = c.XrEvent.Event, ItemValue = c.XrEvent.Id.ToString() })
                        .Distinct().OrderBy(o => o.ItemText).ToList()
                    , "ItemValue", "ItemText");

                filterOptions.AssignedToOptions = new SelectList(db.Incidents.Where(x => x.IsDeleted != true && x.AssignTo != null)
                    .Select(c => new ListValues() { ItemText = c.AssignTo, ItemValue = c.AssignTo })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText");

                filterOptions.FMAOptions = new SelectList(db.Incidents.Where(x => x.IsDeleted != true && x.FMA != null)
                    .Select(c => new ListValues() { ItemText = c.FMA, ItemValue = c.FMA })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText");

                filterOptions.StatusOptions = new SelectList(db.Incidents.Where(x => x.IsDeleted != true && x.XrStatu != null)
                    .Select(c => new ListValues() { ItemText = c.XrStatu.Status, ItemValue = c.XrStatu.Status })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText");

                filterOptions.RegionOptions = new SelectList(db.Incidents.Where(x => x.IsDeleted != true && x.XrEvent != null)
                    .Select(c => new ListValues() { ItemText = c.XrEvent.Region.RegionName, ItemValue = c.XrEvent.Region.RegionName })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText");

                filterOptions.CustomerImpactOptions = new SelectList(db.Incidents.Where(x => x.IsDeleted != true && x.CustomerImpact != null)
                    .Select(c => new ListValues() { ItemText = c.CustomerImpact, ItemValue = c.CustomerImpact })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText");
            }
            else
            {
                filterOptions.EventOptions = new SelectList(db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true)
                    .Select(c => new ListValues() { ItemText = c.XrEvent.Event, ItemValue = c.XrEvent.Id.ToString() })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText", eventId);

                filterOptions.AssignedToOptions = new SelectList(db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true && x.AssignTo != null)
                    .Select(c => new ListValues() { ItemText = c.AssignTo, ItemValue = c.AssignTo })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText", assignedTo);

                filterOptions.FMAOptions = new SelectList(db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true && x.FMA != null)
                    .Select(c => new ListValues() { ItemText = c.FMA, ItemValue = c.FMA })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText", fma);

                filterOptions.StatusOptions = new SelectList(db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true && x.XrStatu != null)
                    .Select(c => new ListValues() { ItemText = c.XrStatu.Status, ItemValue = c.XrStatu.Status })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText", status);

                filterOptions.RegionOptions = new SelectList(db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true && x.XrEvent.Region != null)
                    .Select(c => new ListValues() { ItemText = c.XrEvent.Region.RegionName, ItemValue = c.XrEvent.Region.RegionName })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText");

                filterOptions.CustomerImpactOptions = new SelectList(db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true && x.CustomerImpact != null)
                    .Select(c => new ListValues() { ItemText = c.CustomerImpact, ItemValue = c.CustomerImpact })
                    .Distinct().OrderBy(o => o.ItemText).ToList(), "ItemValue", "ItemText", customerImpact);
            }

            return filterOptions;

        }

        public IEnumerable<SelectListItem> GetListOfAssignedTosByEventId(int eventId, EventMgr eventMgr)
        {
            List<SelectListItem> selectListItemsOfAssignedTos = new List<SelectListItem>();
            SelectListItem selectListItem;

            List<string> listOfAssignedTos;

            if (eventId == 0)
            {
                listOfAssignedTos = db.Incidents.Where(x => x.IsDeleted != true && x.AssignTo != null).Select(c => c.AssignTo).Distinct().ToList();
            }
            else
            {
                listOfAssignedTos = db.Incidents.Where(x => x.EventId == eventId && x.IsDeleted != true && x.AssignTo != null).Select(c => c.AssignTo).Distinct().ToList();
            }

            int indexer = 1;

            selectListItem = new SelectListItem()
            {
                Text = "All",
                Value = "0"
            };

            selectListItemsOfAssignedTos.Add(selectListItem);

            foreach (var item in listOfAssignedTos)
            {
                selectListItem = new SelectListItem
                {
                    Text = item,
                    Value = indexer.ToString()
                };

                selectListItemsOfAssignedTos.Add(selectListItem);

                ++indexer;
            }

            return selectListItemsOfAssignedTos;
        }

        public void MarkIncidentRecordDeleted(int id)
        {
            db.Incidents.First(x => x.IncidentID == id).IsDeleted = true;
            db.SaveChanges();
        }

        public string GetCityName(int regionId, string indexValue)
        {

            if (regionId != 0)
            {
                string csg_Name = _RegionsMgr.GetRegion(regionId).CSG_Name;

                List<SelectListItem> listOfFilteredFmas = CreateFMAList(csg_Name);

                foreach (var item in listOfFilteredFmas)
                {
                    if (item.Value == indexValue)
                    {
                        return item.Text;
                    }
                }
            }
            return "";
        }

        private List<SelectListItem> CreateFMAList(string regionCSGName)
        {
            HttpContext.Current.Response.Cookies["fma"].Expires = DateTime.Now.AddDays(-1);

            List<string> fmaCitiesByRegion;

            List<SelectListItem> listOfFmas = new List<SelectListItem>();

            listOfFmas.Add(new SelectListItem() { Value = "0", Text = "All" });

            if (!string.IsNullOrEmpty(regionCSGName))
            {
                fmaCitiesByRegion = _LocationMgr.GetFMACitiesByRegion(regionCSGName);

                int index = 1;

                foreach (var Item in fmaCitiesByRegion)
                {
                    SelectListItem selectedListItem = new SelectListItem() { Text = Item, Value = index.ToString() };

                    ++index;

                    listOfFmas.Add(selectedListItem);
                }
            }

            StringBuilder sblistOfFilteredFmas = new StringBuilder();

            foreach (var itemValue in listOfFmas)
            {
                sblistOfFilteredFmas.Append(itemValue.Value + "=");
                sblistOfFilteredFmas.Append(itemValue.Text + "+");
            }

            string filteredFmas = sblistOfFilteredFmas.ToString();

            //ManageCookies.SetValue("ListOfFmas", filteredFmas);

            return listOfFmas;
        }

    }
}
