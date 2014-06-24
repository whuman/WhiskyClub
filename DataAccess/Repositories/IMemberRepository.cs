using System;
using System.Collections.Generic;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface IMemberRepository : IDisposable
    {
        Member GetMember(int memberId);

        List<Member> GetAllMembers();

        Member InsertMember(string name);

        bool UpdateMember(int memberId, string name);

        bool DeleteMember(int memberId);
    }
}
