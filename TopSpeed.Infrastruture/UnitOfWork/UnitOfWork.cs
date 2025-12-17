using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Infrastructure.Common;
using TopSpeed.Infrastructure.Repositories;

namespace TopSpeed.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDBContext _dBContext;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public UnitOfWork(ApplicationDBContext dBContext, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Brand = new BrandRepository(_dBContext);
            VehicleType = new VehicleRepository(_dBContext);
            Post = new PostRepository(_dBContext);
          
        }
        public IBrandRepository Brand { get; private set; }

        public IVehicleTypeRepository VehicleType { get; private set; }

        public IPostRepository Post {  get; private set; }  

        public void Dispose()
        {
            _dBContext.Dispose();
        }

        public async Task SaveAsync()
        {
            _dBContext.SaveCommonFields(_userManager, _httpContextAccessor);
            await _dBContext.SaveChangesAsync();
        }
    }
}
