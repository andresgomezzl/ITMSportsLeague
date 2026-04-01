using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task<bool> ExistsByNameAsync(string name); //lo añadí para validar nombre único
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
    }
}
