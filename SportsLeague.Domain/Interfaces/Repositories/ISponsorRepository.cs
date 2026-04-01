using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ISponsorRepository : IGenericRepository<Sponsor>
    {
        Task<Sponsor?> GetByNameAsync(string name);
        Task<IEnumerable<Sponsor>> GetByCityAsync(string city);
    }
}
