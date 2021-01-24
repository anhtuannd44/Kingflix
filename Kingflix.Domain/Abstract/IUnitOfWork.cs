using System;

namespace Kingflix.Domain.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Save changes to database.
        /// </summary>
        void SaveChanges();
    }
}
