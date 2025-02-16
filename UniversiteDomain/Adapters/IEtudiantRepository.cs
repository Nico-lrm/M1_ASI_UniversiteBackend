using UniversiteDomain.Entities;
 
namespace UniversiteDomain.Adapters;
 
public interface IEtudiantRepository : IRepository<Etudiant>
{
    public Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
}