using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Domain.Models;
using TopSpeed.Infrastructure.Common;

namespace TopSpeed.Infrastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDBContext dBContext) :base(dBContext)
        { 

        }
        public async Task  Update(Brand brand)
        {
            //_dbCantext => access to GenericRepository
            var objFromDb = await _dbContext.Brand.FirstOrDefaultAsync(x => x.Id == brand.Id);

            if (objFromDb != null) 
            { 
                 objFromDb.Name = brand.Name;
                 objFromDb.Establishyear = brand.Establishyear;

                if (brand.BrandLogo != null)
                {
                    objFromDb.BrandLogo = brand.BrandLogo;  
                }
            }
            _dbContext.Update(objFromDb);   
        }
    }
}
