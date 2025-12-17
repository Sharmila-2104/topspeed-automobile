using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Domain.Common;
using TopSpeed.Infrastructure.Common;


namespace TopSpeed.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        protected readonly  ApplicationDBContext _dbContext;

        public GenericRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task Create(T entity)
        {
            await _dbContext.AddAsync(entity);
        }

        public async Task Delete(T entity)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();    
        }

        public async Task<List<T>> Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<bool> IsRecordsExsits(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
           return await _dbContext.Set<T>().Where(predicate).AnyAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(X => X.Id == id);    
        }

        public  IEnumerable<T> Query(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
           var entities =  _dbContext.Set<T>().Where(predicate).ToList();

            return entities;
        }

        public IEnumerable<T> Query()
        {
            var entities = _dbContext.Set<T>().AsNoTracking().ToList();

            return entities;
        }
    }
}
