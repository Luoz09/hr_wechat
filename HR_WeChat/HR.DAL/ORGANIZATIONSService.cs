 using Sgms.Frame.EDM.DAL;
using HR.Models; 


namespace HR.DAL
{
    public class ORGANIZATIONSService : EDMOperationService<ORGANIZATIONS>
    {


        public override void DeleteToCache(string id)
        {
            ORGANIZATIONS entity = System.Activator.CreateInstance<ORGANIZATIONS>();
            entity.GUID = id;
            DeleteToCache(entity);
        }
    }
}
