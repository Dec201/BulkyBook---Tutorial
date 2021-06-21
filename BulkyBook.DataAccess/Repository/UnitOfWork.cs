using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _applicationDbContext;


        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            Category = new CategoryRepository(_applicationDbContext);
            CoverType = new CoverTypeRepository(_applicationDbContext);
            SP_Call = new SP_Call(_applicationDbContext);
            Product = new ProductRepository(_applicationDbContext);
            Company = new CompanyRepository(_applicationDbContext);
            ApplicationUser = new ApplicationUserRepository(_applicationDbContext);
            ShoppingCart = new ShoppingCartRepository(_applicationDbContext);
            OrderHeader = new OrderHeaderRepository(_applicationDbContext);
            OrderDetails = new OrderDetailsRepository(_applicationDbContext);
        }


        public ICategoryRepository Category { get; private set; }

        public ICoverTypeRepository CoverType { get; private set; }

        public IProductRepository Product { get; private set; }

        public ICompanyRepository Company { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public ISP_Call SP_Call { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IOrderDetailsRepository OrderDetails { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public void Dispose()
        {
            _applicationDbContext.Dispose();
        }

        public void Save()
        {
            _applicationDbContext.SaveChanges();
        }

    }
}
