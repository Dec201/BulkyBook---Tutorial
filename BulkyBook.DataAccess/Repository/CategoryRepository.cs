using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : RepositoryAsync<Category>, ICategoryRepository
    {

        private readonly ApplicationDbContext _applicationDbContext;


        public CategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Update(Category category)
        {

            var objFromDb = _applicationDbContext.Categories.FirstOrDefault(x => x.Id == category.Id);


            if(objFromDb != null)
            {
                objFromDb.Name = category.Name;
            }

        }
    }
}
