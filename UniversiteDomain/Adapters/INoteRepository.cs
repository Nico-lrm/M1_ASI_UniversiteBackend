using UniversiteDomain.Entities;
 
namespace UniversiteDomain.Adapters;
 
public interface INoteRepository : IRepository<Note>
{
    Task<Note> AddNoteAsync(long idUe, long idEtudiant, float note);
}