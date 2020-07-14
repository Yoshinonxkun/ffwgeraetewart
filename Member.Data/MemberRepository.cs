using System;
using System.Collections.Generic;
using System.Linq;
using Member.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Member.Data
{
    public class MemberRepository : IMemberRepository
    {
        private readonly DatabaseContext _databaseContext;
        private bool _disposed;

        /*
         * ******************************************
         */

        public MemberRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Member> GetMembers()
        {
            return _databaseContext.Members
                .Where(e => e.IsDeleted == false)
                .OrderBy(e => e.Surname)
                .ThenBy(e => e.Name)
                .ToList();
        }

        public Member GetMemberById(int memberId)
        {
            return _databaseContext.Members.Find(memberId);
        }

        public void InsertMember(Member member)
        {
            member.IsDeleted = false;
            member.Psa = new Psa {PsaId = member.MemberId, HelmDate = DateTime.Now, IsDeleted = false};
            _databaseContext.Members.Add(member);
            _databaseContext.SaveChanges();
        }

        public void UpdateMember(Member member)
        {
            _databaseContext.Entry(member).State = EntityState.Modified;
            _databaseContext.SaveChanges();
        }

        public void DeleteMember(Member member)
        {
            member.IsDeleted = true;
            member.Psa.IsDeleted = true;
            _databaseContext.Entry(member).State = EntityState.Modified;
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