using UniversiteDomain.Entities;

namespace UniversiteDomain.Adapters;

public interface IUniversiteRoleRepository : IRepository<IUniversiteRole>
{
    Task AddRoleAsync(string role);
}