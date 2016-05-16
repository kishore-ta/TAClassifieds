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

        //Classified Repository
        private GenericRepository<Classified> _classifiedRepository;
        public GenericRepository<Classified> ClassifiedRepository
        {
            get
            {
                if (this._classifiedRepository == null)
                {
                    this._classifiedRepository = new GenericRepository<Classified>(_context);
                }
                return _classifiedRepository;
            }
        }

        //ClassifiedContact Repository
        private GenericRepository<ClassifiedContact> _classifiedContactRepository;
        public GenericRepository<ClassifiedContact> ClassifiedContactRepository
        {
            get
            {
                if (this._classifiedContactRepository == null)
                {
                    this._classifiedContactRepository = new GenericRepository<ClassifiedContact>(_context);
                }
                return _classifiedContactRepository;
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

        public int Save()
        {
            try
            {
                _context.Configuration.ValidateOnSaveEnabled = false;
                return _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
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
