﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalTour.Data.Abstract
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Table { get; }
        IQueryable<T> GetDataQueryable(Expression<Func<T, bool>> expression = null);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> expression);
        Task Commit();
        Task<T> GetById(object id);
        Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression = null);
        Task Insert(IEnumerable<T> entities);
        Task Insert(T entity);
        IQueryable<T> GetAll();
    }
}
