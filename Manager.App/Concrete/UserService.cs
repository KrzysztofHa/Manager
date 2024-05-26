using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Concrete
{
    public class UserService : BaseService<User>, IUserService, IService<User>
    {
        public string UserName { get;}
        public string DisplayUserName { get; set; }
        public UserService()
        {
            UserName = Environment.UserName;
            
            if (GetAllItem().Any())
            {
                var activeUser = GetAllItem().FirstOrDefault(p => p.UserName == UserName);
                if (activeUser == null)
                {
                    User newUser = new User() { UserName = UserName };
                    AddItem(newUser);
                    SaveList();
                }                
            }
            else
            {
                User newUser = new User() { UserName = UserName };
                AddItem(newUser);
                SaveList();
            }
        }
        public int GetIdActiveUser()
        {
             var activeUser = GetAllItem().FirstOrDefault(p => p.UserName == UserName);
                if (activeUser == null)
                {
                    return 0;
                }
            return activeUser.Id;
        }

        public string GetDisplayUserName()
        {            
            var activeUser = GetAllItem().First(p => p.UserName == UserName);            
            return activeUser.DisplayName;
        }

        public string GetUserName()
        {
            
            throw new NotImplementedException();
        }

        public string SetDisplayUserName(string displayName)
        {
            var activeUser = GetAllItem().FirstOrDefault(p => p.UserName == UserName);
            if (!string.IsNullOrEmpty(displayName) && activeUser != null)
            {
                activeUser.DisplayName = displayName;               
            }
            else
            {
                activeUser.DisplayName = string.Empty;
            }
            SaveList ();
            return activeUser.DisplayName;
        }        
    }
}
