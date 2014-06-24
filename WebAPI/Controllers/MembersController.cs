using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using DAL = WhiskyClub.DataAccess.Models;
using API = WhiskyClub.WebAPI.Models;

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
        public IHttpActionResult Get()
        {
            var members = from h in MemberRepository.GetAllMembers()
                          select new API.Member
                                     {
                                         MemberId = h.MemberId,
                                         Name = h.Name
                                     };

            return Ok(members);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var memberModel = MemberRepository.GetMember(id);
                var item = new API.Member
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

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]API.Member member)
        {
            if (member == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newMember = MemberRepository.InsertMember(member.Name);

            if (newMember != null)
            {
                member.MemberId = newMember.MemberId;

                return Created<API.Member>(string.Format("{0}/{1}", Request.RequestUri, member.MemberId), member);
            }
            else
            {
                return Conflict();
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody]API.Member member)
        {
            if (member == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != member.MemberId)
            {
                return BadRequest("MemberId does not match");
            }

            // TODO : Instead of testing a return value here, we could simply throw exceptions from Repo
            var status = MemberRepository.UpdateMember(id, member.Name);
            if (status)
            {
                //return new HttpResponseMessage(HttpStatusCode.OK);
                return Ok();
            }
            else
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
                return NotFound();
            }
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            var status = MemberRepository.DeleteMember(id);
            if (status)
            {
                //return new HttpResponseMessage(HttpStatusCode.OK);
                return Ok();
            }
            else
            {
                //throw new HttpResponseException(HttpStatusCode.Conflict);
                return Conflict();
            }
        }
    }
}
