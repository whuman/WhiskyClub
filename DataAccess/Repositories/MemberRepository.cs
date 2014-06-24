using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiskyClub.DataAccess.Entities;
using WhiskyClub.DataAccess.Repositories;

namespace WhiskyClub.DataAccess.Repositories
{
    public class MemberRepository : EntityFrameworkRepositoryBase, IMemberRepository
    {
        public Models.Member GetMember(int memberId)
        {
            var entity = GetOne<Member, int>(memberId);

            return new Models.Member
                       {
                           MemberId = entity.MemberId,
                           Name = entity.Name
                       };
        }

        public List<Models.Member> GetAllMembers()
        {
            var hostItems = from e in GetAll<Member>()
                            select new Models.Member
                                       {
                                           MemberId = e.MemberId,
                                           Name = e.Name
                                       };

            return hostItems.ToList();
        }
        
        public Models.Member InsertMember(string name)
        {
            throw new NotImplementedException();
        }

        public bool UpdateMember(int memberId, string name)
        {
            throw new NotImplementedException();
        }
    }
}
