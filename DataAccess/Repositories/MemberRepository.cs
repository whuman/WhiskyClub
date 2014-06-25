using System;
using System.Collections.Generic;
using System.Linq;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Repositories
{
    public class MemberRepository : EntityFrameworkRepositoryBase, IMemberRepository
    {
        public Models.Member GetMember(int memberId)
        {
            var member = GetOne<Member, int>(memberId);

            return new Models.Member
                       {
                           MemberId = member.MemberId,
                           Name = member.Name
                       };
        }

        public List<Models.Member> GetAllMembers()
        {
            var items = from member in GetAll<Member>()
                        select new Models.Member
                                   {
                                       MemberId = member.MemberId,
                                       Name = member.Name
                                   };

            return items.ToList();
        }

        public Models.Member InsertMember(string name)
        {
            try
            {
                var member = new Member();
                member.Name = name;
                member.InsertedDate = DateTime.Now;
                member.UpdatedDate = DateTime.Now;

                Insert(member);

                CommitChanges();

                return new Models.Member
                           {
                               MemberId = member.MemberId,
                               Name = member.Name
                           };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateMember(int memberId, string name)
        {
            try
            {
                var member = GetOne<Member, int>(memberId);
                member.Name = name;
                member.UpdatedDate = DateTime.Now;

                Update(member);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool DeleteMember(int memberId)
        {
            try
            {
                var member = new Member();
                member.MemberId = memberId;

                Delete(member);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
