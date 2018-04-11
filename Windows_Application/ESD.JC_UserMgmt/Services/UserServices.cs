using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace ESD.JC_UserMgmt.Services
{
    public class UserServices : IUserServices
    {
        private IUserRepository userRepository;
        private IGRTransactionRepository grTrnxRepository;
        private IGITransactionRepository giTrnxRepository;

        public UserServices(IUserRepository _userRepository, IGRTransactionRepository grTrnxRepository, IGITransactionRepository giTrnxRepository)
        {
            userRepository = _userRepository;
            this.grTrnxRepository = grTrnxRepository;
            this.giTrnxRepository = giTrnxRepository;
        }

        public IEnumerable<User> GetAll(bool eagerLoading)
        {
            if (eagerLoading)
                return userRepository.GetAll(true);
            else
                return userRepository.GetAll(false);
        }

        public User GetUser(long ID)
        {
            return userRepository.GetUser(ID);
        }

        public bool Save(List<User> users, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    users.ForEach(x => userRepository.Add(x));
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    users.ForEach(x => userRepository.Update(x));
                }
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

        public bool Delete(long ID)
        {
            try
            {
                var userObj = GetUser(ID);
                if (userObj != null)
                {
                    userRepository.Delete(ID);
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

            return true;
        }

        public IEnumerable<GRTransaction> GetUserGRTrnx(long ID)
        {
            try
            {
                var trnx = grTrnxRepository.GetAll(true);
                if (trnx != null)
                {
                    return trnx.Where(u => u.CreatedBy == ID.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return null;
        }

        public IEnumerable<GITransaction> GetUserGITrnx(long ID)
        {
            try
            {
                var trnx = giTrnxRepository.GetAll(true);
                if (trnx != null)
                {
                    return trnx.Where(u => u.CreatedBy == ID.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return null;
        }
    }
}
