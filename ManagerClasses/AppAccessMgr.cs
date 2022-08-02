using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Proof.Core.ManagerClasses
{
    public class AppAccessMgr
    {
        private readonly ProofEntities db = new ProofEntities();

        public AppAccessMgr() { }

        public IEnumerable<AppAccess> GetAppAccess()
        {            
            return db.AppAccesses;
        }

        public AppAccess GetAppAccess(int id)
        {
            return db.AppAccesses.Find(id);
        }

        public AppAccess GetAppAccessByLogin(string ntLogin)
        {
            return db.AppAccesses.Where(x => x.NTLogon == ntLogin).FirstOrDefault();
        }

        public void Edit(AppAccess appAccess)
        {
            db.Entry(appAccess).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Create(AppAccess appAccess)
        {
            db.Entry(appAccess).State = EntityState.Added;
            db.SaveChanges();
        }

    }
}
