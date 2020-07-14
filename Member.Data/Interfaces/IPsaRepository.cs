using System;

namespace Member.Data.Interfaces
{
    public interface IPsaRepository : IDisposable
    {
        Psa GetPsaById(int id);
        Psa GetPsaByMember(Member member);
        void UpdatePsa(Psa psa);
    }
}