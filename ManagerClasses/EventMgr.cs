using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Proof.Core.ManagerClasses
{
    public class EventMgr
    {
        private readonly ProofEntities db = new ProofEntities();

        public EventMgr() { }

        public IEnumerable<XrEvent> GetEvents(bool includeNonActive)
        {
            if (includeNonActive)
            {
                return db.XrEvents.OrderBy(x => x.OrderBy).ThenBy(x => x.Event);
            }
            else
            {
                return db.XrEvents.Where(a => a.Active == true).OrderBy(x => x.OrderBy).ThenBy(x => x.Event);
            }
        }

        public XrEvent GetEvent(int id)
        {
            return db.XrEvents.Find(id);
        }

        public void Create(XrEvent xrEvent)
        {
            db.XrEvents.Add(xrEvent);
            db.SaveChanges();
        }

        public void Edit(XrEvent xrEvent)
        {
            db.Entry(xrEvent).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<int> GetOrderBys()
        {
            List<int> listOfOrderBys = new List<int>();

            for(int index = 1; index < 5; ++index)
            {
                listOfOrderBys.Add(index);
            }

            return listOfOrderBys;
        }

        public bool IsThisDuplicateEventForThisRegion(string eventName, int regionId, int eventId)
        {
            return db.XrEvents.Any(x => x.Event.ToUpper() == eventName.ToUpper() && x.RegionId == regionId && x.Id != eventId);
        }


    public IEnumerable<string> GetFMAforAll(bool includeNonActive)
    {
        IEnumerable<string> xrEvents;

        if (includeNonActive)
        {
            return xrEvents = (from x in db.XrEvents
                               join i in db.Incidents
                             on x.Id equals i.EventId
                               select i.FMA).Distinct().ToList();
        }
        else
        {
            return xrEvents = (from x in db.XrEvents
                               join i in db.Incidents
                             on x.Id equals i.EventId
                               where x.Active == true
                               select  i.FMA).Distinct().ToList();
        }
    }

        public IEnumerable<string> GetAssignToforAll(bool includeNonActive)
        {
            IEnumerable<string> xrEvents;

            if (includeNonActive)
            {
                return xrEvents = (from x in db.XrEvents
                                   join i in db.Incidents
                                 on x.Id equals i.EventId
                                   select i.AssignTo).Distinct().ToList();
            }
            else
            {
                return xrEvents = (from x in db.XrEvents
                                   join i in db.Incidents
                                 on x.Id equals i.EventId
                                   where x.Active == true
                                   select i.AssignTo).Distinct().ToList();
            }
        }



    }
}
