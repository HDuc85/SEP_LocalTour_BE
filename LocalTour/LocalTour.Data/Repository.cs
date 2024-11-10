using LocalTour.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace LocalTour.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        LocalTourDbContext _context;
        public Repository(LocalTourDbContext localTourDbContext)
        {
            _context = localTourDbContext;
        }
        public IQueryable<T> GetDataQueryable(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
            {
                return _context.Set<T>();
            }
            return _context.Set<T>().Where(expression);
        }

        public async Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
            {
                return await _context.Set<T>().ToListAsync();
            }
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Delete(T entity)
        {
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
            var entities = _context.Set<T>().Where(expression).ToList();
            if (entities.Count > 0)
            {
                _context.Set<T>().RemoveRange(entities);
            }
        }

        public async Task Insert(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task Insert(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        public virtual IQueryable<T> Table => _context.Set<T>();
        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }
        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }
    }
}
