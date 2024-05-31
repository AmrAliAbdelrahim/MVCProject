using Demo.BLL.Interfaces;
using Demo.DAL.Data;
using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private protected readonly AppDbContext _dbContext;
        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
           => _dbContext.Add(entity);
            //_dbContext.Set<T>().Add(entity);
             
        

        public void Delete(T entity)
       
         =>   _dbContext.Remove(entity);
            //_dbContext.Set<T>().Remove(entity);
           
      
        public void Update(T entity)
        
           => _dbContext.Update(entity);
            //_dbContext.Set<T>().Update(entity);
             
        
        public IEnumerable<T> GetAll()
        {
            if(typeof(T)==typeof(Employee))
                return (IEnumerable<T>) _dbContext.Employees.Include(E=>E.Department).AsNoTracking().ToList();
            else
                return _dbContext.Set<T>().AsNoTracking().ToList();
        }

        public T GetById(int id)
        {
            //return _dbContext.Employees.Find(id);
            return _dbContext.Find<T>(id);
            ///var Employee = _dbContext.Employees.Local.Where(D => D.Id == id).FirstOrDefault();
            ///if (Employee == null)
            ///    Employee = _dbContext.Employees.Where(D=>D.Id == id ).FirstOrDefault();
            ///return Employee;

        }
    }
}
