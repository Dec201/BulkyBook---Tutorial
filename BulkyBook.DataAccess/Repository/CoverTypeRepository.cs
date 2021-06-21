using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {


        private readonly ApplicationDbContext _applicationDbContext;

        public CoverTypeRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;

        }





        public void Update(CoverType coverType)
        {

            var objFromDb = _applicationDbContext.CoverTypes.FirstOrDefault(x => x.Id == coverType.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = coverType.Name;
            }


        }
    }
}
