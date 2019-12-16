 using Sgms.Frame.EDM.DAL;
using HR.Models; 


namespace HR.DAL
{
    public class OU_USERSService : EDMOperationService<OU_USERS>
    {

        public override void DeleteToCache(string id)
        {
            OU_USERS entity = System.Activator.CreateInstance<OU_USERS>();
            entity.USER_GUID = id;
            DeleteToCache(entity);
        }

    }
}
