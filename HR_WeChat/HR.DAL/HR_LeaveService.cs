using JidaIT.Models;
using Sgms.Frame.EDM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JidaIT.DAL
{
    public class HR_LeaveService : EDMOperationService<HR_Leave>
    { 
        public override  void DeleteToCache(string id)
        {
            HR_Leave entity = System.Activator.CreateInstance<HR_Leave>();
            entity.ID = id;
            entity.Guid = id;
            DeleteToCache(entity);
        }
         
    }
}
