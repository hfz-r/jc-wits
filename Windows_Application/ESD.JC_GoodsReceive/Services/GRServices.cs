using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace ESD.JC_GoodsReceive.Services
{
    public class GRServices : IGRServices
    {
        private IGoodsReceiveRepository grRepository;

        public GRServices(IGoodsReceiveRepository grRepository)
        {
            this.grRepository = grRepository;
        }

        public IEnumerable<GoodsReceive> GetAll()
        {
            return grRepository.GetAll(false);
        }

        public GoodsReceive GetGR(long ID)
        {
            return grRepository.GetGR(ID);
        }

        public GoodsReceive GetSAPNo(string sap_no)
        {
            return grRepository.GetSAPNo(sap_no);
        }

        public bool Save(List<GoodsReceive> grs, string state = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(state) && state == "Save")
                {
                    grs.ForEach(x => grRepository.Add(x));
                }
                else if (!string.IsNullOrEmpty(state) && state == "Update")
                {
                    grs.ForEach(x => grRepository.Update(x));
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
    }
}
