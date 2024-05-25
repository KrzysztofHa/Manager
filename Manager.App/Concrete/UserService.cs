﻿using Manager.App.Abstract;
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
            LoadList();
            if (GetAllItem().Any())
            {
                var activeUser = GetAllItem().FirstOrDefault(p => p.UserName == this.UserName);
                if (activeUser != null)
                {
                    User newUser = new User() { UserName = this.UserName };
                    AddItem(newUser);
                }
            }
            else
            {
                User newUser = new User() { UserName = this.UserName };
                AddItem(newUser);
                SaveList();
            }
        }
        public int GetIdActiveUser()
        {

            return 1;//activeUser.Id;
        }

        public string GetDisplayUserName()
        {            
            var activeUser = GetAllItem().FirstOrDefault(p => p.UserName == this.UserName);            
            return activeUser.DisplayName;
        }

        public string GetUserName()
        {
            
            throw new NotImplementedException();
        }

        public void SetDisplayUserName(string displayName)
        {
            throw new NotImplementedException();
        }        
    }
}
