using System;
using System.Collections.Generic;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface IHostRepository : IDisposable
    {
        Host GetHost(int hostId);

        List<Host> GetAllHosts();
    }
}
