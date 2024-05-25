using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface IUserService
    {
        public int GetIdActiveUser();
        public string GetDisplayUserName();        
        public string GetUserName();
        public void SetDisplayUserName(string displayName);
    }    
}
