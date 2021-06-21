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
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly ApplicationDbContext _applicationDbContext;


        public ProductRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Update(Product product)
        {

            var objFromDb = _applicationDbContext.Products.FirstOrDefault(x => x.Id == product.Id);


            if(objFromDb != null)
            {

                if(product.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }


                objFromDb.Title = product.Title;
                objFromDb.Description = product.Description;
                objFromDb.Author = product.Author;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.CoverTypeId = product.CoverTypeId;
                objFromDb.ISBN = product.ISBN;
                objFromDb.ListPrice = product.ListPrice;
                objFromDb.Price= product.Price;
                objFromDb.Price50= product.Price50;
                objFromDb.Price100 = product.Price100;



            }

        }
    }
}
