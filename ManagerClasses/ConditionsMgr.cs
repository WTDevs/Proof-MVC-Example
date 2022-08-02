using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace Proof.Core.ManagerClasses
{
    public class ConditionsMgr
    {
        private readonly ProofEntities db = new ProofEntities();

        public ConditionsMgr() { }

        public IEnumerable<XrCondition> GetConditions(bool includeNonActive)
        {
            return includeNonActive ? db.XrConditions : db.XrConditions.Where(x => x.Active == true);
        }

        public XrCondition GetCondition(int conditionId)
        {
            return db.XrConditions.Where(x => x.Id == conditionId).FirstOrDefault();
        }

        public void Create(XrCondition xrCondition)
        {
            db.XrConditions.Add(xrCondition);
            db.SaveChanges();
        }

        public XrCondition GetConditionByRegionIdAndConditionName(int regionId, string conditionName)
        {
            return db.XrConditions.Where(x => x.RegionId == regionId && x.Conditions == conditionName).FirstOrDefault();
        }

        public IEnumerable<XrCondition> GetConditionsByRegionId(int regionId)
        {
            return db.XrConditions.Where(x => x.RegionId == regionId && x.Active == true);
        }

        public void Edit(XrCondition xrCondition)
        {
            db.Entry(xrCondition).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<int> GetOrderBys()
        {
            List<int> listOfOrderBys = new List<int>();

            for (int index = 1; index < 5; ++index)
            {
                listOfOrderBys.Add(index);
            }

            return listOfOrderBys;
        }

        public bool IsThisDuplicateConditionForThisRegion(string condition, int regionId, int conditionId)
        {
            return db.XrConditions.Any(x => x.Conditions.ToUpper() == condition.ToUpper() && x.RegionId == regionId && x.Id != conditionId);
        }

    }
}
