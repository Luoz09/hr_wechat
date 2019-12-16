 using Sgms.Frame.EDM.DAL;
using HR.Models; 


namespace HR.DAL
{
    public class USERSService : EDMOperationService<USERS>
    {
        public override void DeleteToCache(string id)
        {
            USERS entity = System.Activator.CreateInstance<USERS>();
            entity.GUID = id;
            DeleteToCache(entity);
        }

    }
}
