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
        if (idUe <= 0)
            throw new ArgumentOutOfRangeException(nameof(idUe), "L'id de l'UE doit être supérieur à 0.");
        if (idEtudiant <= 0)
            throw new ArgumentOutOfRangeException(nameof(idEtudiant), "L'id de l'étudiant doit être supérieur à 0.");
        // Note : 0 est une note valide, on vérifie donc seulement si la note est négative.
        if (note < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(note), "La note ne peut pas être négative.");

        // Vérifions que les repositories ne sont pas nuls
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());

        // Check 1 : La note doit être comprise entre 0 et 20
        if (note < 0.0f || note > 20.0f)
            throw new NoteOutOfRangeException("La note doit être comprise entre 0 et 20");

        // Vérifier que l'UE et l'étudiant existent
        List<Etudiant> etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id == idEtudiant);
        if (etudiants.Count == 0)
            throw new EtudiantNotFoundException("AddNoteEtudiantUeUseCase : L'étudiant n'existe pas");
        Etudiant etudiant = etudiants.First();
        
        List<Ue> ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.Id == idUe);
        if (ues.Count == 0)
            throw new UeNotFoundException("AddNoteEtudiantUeUseCase : L'UE n'existe pas");
        Ue ue = ues.First();

        // Check 2 : Un étudiant ne peut avoir une note que dans une UE appartenant au parcours dans lequel il est inscrit.
        if (etudiant.ParcoursSuivi == null)
            throw new InscriptionNotFoundException("AddNoteEtudiantUeUseCase : L'étudiant n'est inscrit à aucun parcours");
        Parcours parcours = etudiant.ParcoursSuivi;
        var associatedUeIds = parcours.UesEnseignees.Select(u => u.Id).ToArray();
        
        if (parcours.UesEnseignees == null || !parcours.UesEnseignees.Any(u => u.Id == idUe))
            throw new InscriptionNotFoundException("AddNoteEtudiantUeUseCase : L'UE n'est pas dans ce parcours");

        // Check 3 : Un étudiant ne peut avoir qu'une seule note par UE.
        var notes = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.idUe == idUe && n.idEtud == idEtudiant);
        if (notes != null && notes.Count > 0)
            throw new DuplicateNoteException("AddNoteEtudiantUeUseCase : Une note existe déjà pour cet étudiant dans cette UE");
    }
}