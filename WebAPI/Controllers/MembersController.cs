using System;
using System.Linq;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class MembersController : ApiController
    {
        private IMemberRepository MemberRepository { get; }

        public MembersController() : this(new MemberRepository()) { }

        public MembersController(IMemberRepository memberRepository)
        {
            if (memberRepository == null)
            {
                throw new ArgumentNullException(nameof(memberRepository));
            }

            MemberRepository = memberRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            var members = from member in MemberRepository.GetAllMembers()
                          select new Member
                                     {
                                         MemberId = member.MemberId,
                                         Name = member.Name
                                     };

            return Ok(members);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var member = MemberRepository.GetMember(id);
                var item = new Member
                               {
                                   MemberId = member.MemberId,
                                   Name = member.Name
                               };

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]Member member)
        {
            if (member == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newMember = MemberRepository.InsertMember(member.Name);

            if (newMember != null)
            {
                member.MemberId = newMember.MemberId;

                return Created($"{Request.RequestUri}/{member.MemberId}", member);
            }
            else
            {
                return Conflict();
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody]Member member)
        {
            if (member == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != member.MemberId)
            {
                return BadRequest("MemberId does not match");
            }

            var status = MemberRepository.UpdateMember(id, member.Name);
            if (status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            var status = MemberRepository.DeleteMember(id);
            if (status)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }
    }
}
