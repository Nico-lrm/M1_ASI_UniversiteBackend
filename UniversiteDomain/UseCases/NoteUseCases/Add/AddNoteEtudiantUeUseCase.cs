using System.Data;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Add;

public class AddNoteEtudiantUeUseCase(IRepositoryFactory repositoryFactory)
{
    // Rajout d'un étudiant dans un parcours
    public async Task<Note> ExecuteAsync(Ue ue, Etudiant etudiant, float note)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(etudiant);
        return await ExecuteAsync(ue.Id, etudiant.Id, note);
    }  
    
    // TODO : Refactor this
    public async Task<Note> ExecuteAsync(long idUe, long idEtudiant, float note)
    {
        await CheckBusinessRules(note, idUe, idEtudiant); 
        return await repositoryFactory.NoteRepository().AddNoteAsync(idEtudiant, idUe, note);
    }
    
    private async Task CheckBusinessRules(float note, long idUe, long idEtudiant)
    {
        // Vérification des paramètres
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // Check 1 : Une note est comprise entre 0 et 20
        if (note < 0.0 || note > 20.0)
        {
            throw new NoteOutOfRangeException("La note doit être comprise entre 0 et 20");
        }
        
        // Vérifier que l'UE et l'étudiant existe au préalable
        List<Etudiant> etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Id.Equals(idEtudiant));
        if (etudiant is { Count: 0 }) throw new EtudiantNotFoundException(idEtudiant.ToString());
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(p=>p.Id.Equals(idUe));
        if (ue is { Count: 0 }) throw new UeNotFoundException(idUe.ToString());
        
        // Check 2 : Un étudiant ne peut avoir une note que dans une Ue du parcours dans lequel il est inscrit
        List<Parcours> parcours = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.Inscrits.Find(inscri => inscri.Id == idEtudiant) != null && p.UesEnseignees.Find(ueEns => ueEns.Id == idUe) != null);
        if (parcours is { Count: 0 }) throw new InscriptionNotFoundException("Aucune inscription trouvé pour cet étudiant et cette UE");
        
        // Check 3 : Un étudiant n'a qu'une note au maximum par Ue
        List<Note> notes = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.idEtud.Equals(idEtudiant) && n.idUe.Equals(idUe));
        if (notes is { Count: 1 }) throw new DuplicateNoteException("Une note existe déjà pour cet étudiant");
    }
}