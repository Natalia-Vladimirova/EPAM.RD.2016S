﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using UserStorage.Interfaces.Criteria;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Services;

namespace WcfServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WcfUserService : MarshalByRefObject, IWcfUserService
    {
        private readonly IUserService userService;

        public WcfUserService(IUserService userService)
        {
            if (userService == null)
            {
                throw new ArgumentNullException($"{nameof(userService)} must be not null.");
            }

            this.userService = userService;
        }

        public int Add(User user)
        {
            return userService.Add(user);
        }

        public void Delete(int personalId)
        {
            userService.Delete(personalId);
        }

        public IList<int> Search(User criteria)
        {
            return userService.Search(u => u.Compare(criteria));
        }
    }
}
