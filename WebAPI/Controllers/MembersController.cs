using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class MembersController : ApiController
    {
        public IMemberRepository MemberRepository { get; set; }

        public MembersController() : this(new MemberRepository()) { }

        public MembersController(IMemberRepository memberRepository)
        {
            if (memberRepository == null)
            {
                throw new ArgumentNullException("memberRepository");
            }

            MemberRepository = memberRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            var members = from h in MemberRepository.GetAllMembers()
                          select new Member
                          {
                              MemberId = h.MemberId,
                              Name = h.Name
                          };

            return Ok(members);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int memberId)
        {
            try
            {
                var memberModel = MemberRepository.GetMember(memberId);
                var item = new Member
                               {
                                   MemberId = memberModel.MemberId,
                                   Name = memberModel.Name
                               };

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        ////// POST api/<controller>
        ////public void Post([FromBody]string value)
        ////{
        ////}

        ////// PUT api/<controller>/5
        ////public void Put(int id, [FromBody]string value)
        ////{
        ////}

        ////// DELETE api/<controller>/5
        ////public void Delete(int id)
        ////{
        ////}
    }
}
