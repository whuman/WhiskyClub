using System;
using System.Collections.Generic;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface IMemberRepository : IDisposable
    {
        Member GetMember(int memberId);

        List<Member> GetAllMembers();

        Member AddMember(Member newMember);

        bool UpdateMember(Member existingMember);
    }
}
