﻿using LocalTour.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        LocalTourDbContext _context;
        public Repository(LocalTourDbContext localTourDbContext)
        {
            _context = localTourDbContext;
        }

        public async Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression)
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
