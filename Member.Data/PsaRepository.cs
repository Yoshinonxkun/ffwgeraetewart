using System;
using Member.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Member.Data
{
    public class PsaRepository : IPsaRepository
    {
        private readonly DatabaseContext _databaseContext;
        private bool _disposed;

        /*
         * ******************************************
         */

        public PsaRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Psa GetPsaById(int id)
        {
            return _databaseContext.Psas.Find(id);
        }

        public Psa GetPsaByMember(Member member)
        {
            return _databaseContext.Psas.Find(member.MemberId);
        }

        public void UpdatePsa(Psa psa)
        {
            _databaseContext.Entry(psa).State = EntityState.Modified;
            _databaseContext.SaveChanges();
        }

        /*
         * ******************************************
         */

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _databaseContext.Dispose();
            _disposed = true;
        }
    }
}