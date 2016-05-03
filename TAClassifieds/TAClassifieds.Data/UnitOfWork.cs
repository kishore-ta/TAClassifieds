using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAClassifieds.Data;
using TAClassifieds.Model;

namespace TAClassifieds.Model
{
    public class UnitOfWork :IDisposable
    {
        private readonly ClassifiedsContext _context = new ClassifiedsContext();
        private GenericRepository<User> _userRepository;
        private GenericRepository<Category> _categoryRepository;
        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this._categoryRepository == null)
                {
                    this._categoryRepository = new GenericRepository<Category>(_context);
                }
                return _categoryRepository;
            }
        }
        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new GenericRepository<User>(_context);
                }
                return _userRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
