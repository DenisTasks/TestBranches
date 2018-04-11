using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelService
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private WPFOutlookContext _context;
        private DbSet<TEntity> _dbSet;

        public GenericRepository(WPFOutlookContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public DbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public IEnumerable<TEntity> Get()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).ToList();
        }      

        public TEntity FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Create(TEntity item)
        {
            _dbSet.Add(item);
        }

        public void Update(TEntity item)
        {
            _dbSet.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
        }

        public void Remove(TEntity item)
        {
            if (_context.Entry(item).State == EntityState.Detached)
            {
                _dbSet.Attach(item);
            }
            _dbSet.Remove(item);
        }

        public void Remove(TEntity entity, Func<TEntity, int> getKey)
        {
            if (entity == null)
            {
                throw new ArgumentException("Cannot remove a null entity.");
            }

            var entry = _context.Entry<TEntity>(entity);

            if (entry.State == EntityState.Detached)
            {
                var set = _context.Set<TEntity>();
                TEntity attachedEntity = set.Find(getKey(entity));

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    _dbSet.Remove(attachedEntry.Entity);
                }
                else
                {
                    _dbSet.Remove(entity);
                }
            }
        }
    }
}
