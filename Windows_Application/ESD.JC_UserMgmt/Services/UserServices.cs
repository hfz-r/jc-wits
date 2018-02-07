using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace ESD.JC_UserMgmt.Services
{
    public class UserServices : IUserServices
    {
        private IUserRepository userRepository;

        public UserServices()
        {
            userRepository = new UserRepository(new InventoryContext());
        }

        public UserServices(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public IEnumerable<User> GetAll()
        {
            return userRepository.GetAll();
        }

        public User GetUser(long? ID)
        {
            return userRepository.GetUser(ID.GetValueOrDefault());
        }

        public bool Save(User User)
        {
            try
            {
                User obj = GetUser(User.ID);
                if (obj == null)
                {
                    userRepository.AddInto(User);
                }
                else
                {
                    userRepository.Update(User);
                }
                userRepository.Save();
            }
            catch (DbEntityValidationException e)
            {
                string exceptionMessage = string.Empty;

                foreach (var eve in e.EntityValidationErrors)
                {
                    exceptionMessage = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        exceptionMessage += string.Format("\n\n- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(exceptionMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public void Delete(long? ID)
        {
            try
            {
                var userObj = GetUser(ID.GetValueOrDefault());
                if (userObj != null)
                {
                    userRepository.Delete(ID.Value);
                }
                else
                {
                    throw new Exception("User Not Found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
