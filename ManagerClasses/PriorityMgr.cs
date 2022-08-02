using System.Collections.Generic;
using System.Linq;

namespace Proof.Core.ManagerClasses
{
    public class PriorityMgr
    {
        private readonly ProofEntities db = new ProofEntities();

        public PriorityMgr() { }

        public IEnumerable<XrPriority> GetPriorities()
        {
            return db.XrPriorities.Where(x => x.Active == true);
        }

    }
}
