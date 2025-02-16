using UniversiteDomain.Adapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note> AddNoteAsync(long idUe, long idEtudiant, float note)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Ue u = (await Context.Ues.FindAsync(idUe));
        
        // Création d'une nouvelle instance de Note
        Note newNote = new Note
        {
            idUe = idUe,
            idEtud = idEtudiant,
            valeur = note
        };
        
        e.NotesObtenues.Add(newNote);
        u.Notes.Add(newNote);
        await Context.Notes.AddAsync(newNote);
        await Context.SaveChangesAsync();
        return newNote;
    }
}