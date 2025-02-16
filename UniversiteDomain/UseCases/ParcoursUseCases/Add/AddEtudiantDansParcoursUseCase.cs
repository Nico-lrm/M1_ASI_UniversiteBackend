using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Add;

public class AddEtudiantDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    // Rajout d'un étudiant dans un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiant);
        return await ExecuteAsync(parcours.Id, etudiant.Id);
    }
    
    public async Task<Parcours> ExecuteAsync(long idParcours, long idEtudiant)
    {
        await CheckBusinessRules(idParcours, idEtudiant);
        
        // Récupérer les entités depuis les repositories
        Etudiant etudiant = await repositoryFactory.EtudiantRepository().FindAsync(idEtudiant)
            ?? throw new EtudiantNotFoundException(idEtudiant.ToString());
        Parcours parcours = await repositoryFactory.ParcoursRepository().FindAsync(idParcours)
            ?? throw new ParcoursNotFoundException(idParcours.ToString());
        
        // Affectation du parcours à l'étudiant
        etudiant.ParcoursSuivi = parcours;
        
        // Initialiser la collection des inscrits dans le parcours si nécessaire
        if (parcours.Inscrits == null)
            parcours.Inscrits = new List<Etudiant>();
        
        // Ajouter l'étudiant dans la collection s'il n'y figure pas déjà
        if (!parcours.Inscrits.Any(e => e.Id == etudiant.Id))
        {
            parcours.Inscrits.Add(etudiant);
        }
        
        // Sauvegarder les mises à jour pour les deux entités
        await repositoryFactory.EtudiantRepository().UpdateAsync(etudiant);
        await repositoryFactory.ParcoursRepository().UpdateAsync(parcours);
        
        return parcours;
    }
    
    // Rajout de plusieurs étudiants dans un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Etudiant> etudiants)
    {
        long[] idEtudiants = etudiants.Select(x => x.Id).ToArray();
        return await ExecuteAsync(parcours.Id, idEtudiants);
    }
    
    public async Task<Parcours> ExecuteAsync(long idParcours, long[] idEtudiants)
    {
        foreach (var id in idEtudiants)
        {
            await CheckBusinessRules(idParcours, id);
            // Pour chaque étudiant, on effectue l'inscription individuelle
            await ExecuteAsync(idParcours, id);
        }
        
        // Retourner le parcours mis à jour
        return await repositoryFactory.ParcoursRepository().FindAsync(idParcours)
            ?? throw new ParcoursNotFoundException(idParcours.ToString());
    }
    
    // Vérification des règles métier
    private async Task CheckBusinessRules(long idParcours, long idEtudiant)
    {
        // Comme idParcours et idEtudiant sont des types valeur, il n'est pas nécessaire d'appeler ThrowIfNull.
        if (idParcours <= 0)
            throw new ArgumentOutOfRangeException(nameof(idParcours), "L'id du parcours doit être supérieur à 0.");
        if (idEtudiant <= 0)
            throw new ArgumentOutOfRangeException(nameof(idEtudiant), "L'id de l'étudiant doit être supérieur à 0.");
        
        // Vérifier que les repositories ne sont pas nuls
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // Vérifier que l'étudiant existe
        List<Etudiant> etudiantList = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id == idEtudiant);
        if (etudiantList == null || etudiantList.Count == 0)
            throw new EtudiantNotFoundException(idEtudiant.ToString());
        
        // Vérifier que le parcours existe
        List<Parcours> parcoursList = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.Id == idParcours);
        if (parcoursList == null || parcoursList.Count == 0)
            throw new ParcoursNotFoundException(idParcours.ToString());
        
        // Vérifier que l'étudiant n'est pas déjà inscrit dans le parcours
        List<Etudiant> inscrit = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id == idEtudiant 
                                       && e.ParcoursSuivi != null 
                                       && e.ParcoursSuivi.Id == idParcours);
        if (inscrit != null && inscrit.Count > 0)
            throw new DuplicateInscriptionException($"{idEtudiant} est déjà inscrit dans le parcours : {idParcours}");
    }
}