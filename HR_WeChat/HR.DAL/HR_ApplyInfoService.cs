 using Sgms.Frame.EDM.DAL;
using HR.Models; 


namespace HR.DAL
{
    public class HR_ApplyInfoService : EDMOperationService<HR_ApplyInfo>
    {

        public override void DeleteToCache(string id)
        {
            HR_ApplyInfo entity = System.Activator.CreateInstance<HR_ApplyInfo>();
            entity.ApplyID = id;
            DeleteToCache(entity);
        }

    }
}
