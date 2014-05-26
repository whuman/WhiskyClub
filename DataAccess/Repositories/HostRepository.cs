using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiskyClub.DataAccess.Entities;
using WhiskyClub.DataAccess.Repositories;

namespace WhiskyClub.DataAccess.Repositories
{
    public class HostRepository : EntityFrameworkRepositoryBase, IHostRepository
    {
        public Models.Host GetHost(int hostId)
        {
            var entity = GetOne<Host, int>(hostId);

            return new Models.Host
                       {
                           HostId = entity.HostId,
                           Name = entity.Name
                       };
        }

        public List<Models.Host> GetAllHosts()
        {
            var hostItems = from e in GetAll<Host>()
                            select new Models.Host
                                       {
                                           HostId = e.HostId,
                                           Name = e.Name
                                       };

            return hostItems.ToList();
        }
    }
}
