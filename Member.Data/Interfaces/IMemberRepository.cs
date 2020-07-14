using System;
using System.Collections.Generic;

namespace Member.Data.Interfaces
{
    public interface IMemberRepository : IDisposable
    {
        IEnumerable<Member> GetMembers();
        Member GetMemberById(int memberId);
        void InsertMember(Member member);
        void UpdateMember(Member member);
        void DeleteMember(Member member);
    }
}