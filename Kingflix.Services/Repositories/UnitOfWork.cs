using Kingflix.Domain.Abstract;
using Kingflix.Services.Data;

namespace Kingflix.Services.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _isDisposed;
        private readonly AppDbContext _context;

        public UnitOfWork(IAppDbContext context)
        {
            _context = context as AppDbContext;
        }

        /// <summary>
        /// Make sure there are no open sessions.
        /// In the web app this will be called when the injected UnitOfWork manager
        /// is disposed at the end of a request.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _context.Dispose();
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Save changes to database.
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
