using System.Data;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Add;

public class AddNoteEtudiantUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(Ue ue, Etudiant etudiant, float note)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(etudiant);
        return await ExecuteAsync(ue.Id, etudiant.Id, note);
    }  
    
    public async Task<Note> ExecuteAsync(long idUe, long idEtudiant, float note)
    {
        await CheckBusinessRules(idUe, idEtudiant, note); 
        return await repositoryFactory.NoteRepository().AddNoteAsync(idUe, idEtudiant, note);
    }
    
    private async Task CheckBusinessRules(long idUe, long idEtudiant, float note)
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
        
        // Vérifier que l'UE et l'étudiant existent au préalable
        List<Etudiant> etudiants = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Id.Equals(idEtudiant));
        if (etudiants is { Count: 0 }) throw new EtudiantNotFoundException("AddNoteEtudiantUeUseCase : L'étudiant n'existe pas");
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(p=>p.Id.Equals(idUe));
        if (ue is { Count: 0 }) throw new UeNotFoundException("AddNoteEtudiantUeUseCase : L'UE n'existe pas");
        
        // Check 2 : Un étudiant ne peut avoir une note que dans une Ue du parcours dans lequel il est inscrit
        List<Parcours> parcours = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.Inscrits.Find(inscrit => inscrit.Id == idEtudiant) != null && p.UesEnseignees.Find(ueEns => ueEns.Id == idUe) != null);
        if (parcours is { Count: 0 }) throw new InscriptionNotFoundException("AddNoteEtudiantUeUseCase : Aucune inscription trouvé pour cet étudiant et cette UE");
        
        // Check 3 : Un étudiant n'a qu'une note au maximum par Ue
        List<Note> notes = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.idUe.Equals(idUe) && n.idEtud.Equals(idEtudiant));
        if (notes is { Count: 1 }) throw new DuplicateNoteException("AddNoteEtudiantUeUseCase : Une note existe déjà pour cet étudiant");
    }
}