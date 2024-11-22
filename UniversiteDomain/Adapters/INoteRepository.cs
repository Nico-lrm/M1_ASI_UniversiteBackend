using UniversiteDomain.Entities;
 
namespace UniversiteDomain.Adapters;
 
public interface INoteRepository : IRepository<Note>
{
    Task<Note> AddNoteAsync(Etudiant etudiant, Ue ue, float note);
    Task<Note> AddNoteAsync(long idEtudiant, long idUe, float note);
}