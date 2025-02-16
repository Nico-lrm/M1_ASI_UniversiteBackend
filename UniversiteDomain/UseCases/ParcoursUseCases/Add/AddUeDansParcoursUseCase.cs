using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Add;

public class AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    // Ajout d'une UE dans un parcours via entités
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ue);
        return await ExecuteAsync(parcours.Id, ue.Id);
    }
    
    // Ajout d'une UE dans un parcours via identifiants
    public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
    {
        await CheckBusinessRules(idParcours, idUe);
        
        // Récupération des entités
        Ue ue = await repositoryFactory.UeRepository().FindAsync(idUe)
            ?? throw new UeNotFoundException(idUe.ToString());
        Parcours parcours = await repositoryFactory.ParcoursRepository().FindAsync(idParcours)
            ?? throw new ParcoursNotFoundException(idParcours.ToString());
        
        // Mise à jour bidirectionnelle :
        // Ajout de l'UE dans la collection du parcours.
        if (parcours.UesEnseignees == null)
            parcours.UesEnseignees = new List<Ue>();
        if (!parcours.UesEnseignees.Any(u => u.Id == ue.Id))
        {
            parcours.UesEnseignees.Add(ue);
        }
        
        // Ajout du parcours dans la collection de l'UE.
        if (ue.EnseigneeDans == null)
            ue.EnseigneeDans = new List<Parcours>();
        if (!ue.EnseigneeDans.Any(p => p.Id == parcours.Id))
        {
            ue.EnseigneeDans.Add(parcours);
        }
        
        // Sauvegarder les modifications sur les deux entités.
        await repositoryFactory.UeRepository().UpdateAsync(ue);
        await repositoryFactory.ParcoursRepository().UpdateAsync(parcours);
        
        return parcours;
    }
    
    // Ajout de plusieurs UEs dans un parcours via entités
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Ue> ues)
    {
        ArgumentNullException.ThrowIfNull(ues);
        ArgumentNullException.ThrowIfNull(parcours);
        long[] idUes = ues.Select(x => x.Id).ToArray();
        return await ExecuteAsync(parcours.Id, idUes);
    }
    
    // Ajout de plusieurs UEs dans un parcours via identifiants
    public async Task<Parcours> ExecuteAsync(long idParcours, long[] idUes)
    {
        foreach (var id in idUes)
        {
            await CheckBusinessRules(idParcours, id);
            await ExecuteAsync(idParcours, id);
        }
        
        return await repositoryFactory.ParcoursRepository().FindAsync(idParcours)
            ?? throw new ParcoursNotFoundException(idParcours.ToString());
    }
    
    // Vérification des règles métier pour l'ajout d'une UE dans un parcours
    private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        // Vérification des paramètres (idParcours et idUe étant des valeurs, on vérifie leur positivité)
        if (idParcours <= 0)
            throw new ArgumentOutOfRangeException(nameof(idParcours), "L'id du parcours doit être supérieur à 0.");
        if (idUe <= 0)
            throw new ArgumentOutOfRangeException(nameof(idUe), "L'id de l'UE doit être supérieur à 0.");
        
        // Vérifier que les repositories nécessaires sont disponibles
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // Vérifier que l'UE existe
        List<Ue> ueList = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.Id == idUe);
        if (ueList == null || ueList.Count == 0)
            throw new UeNotFoundException(idUe.ToString());
        
        // Vérifier que le parcours existe
        List<Parcours> parcoursList = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.Id == idParcours);
        if (parcoursList == null || parcoursList.Count == 0)
            throw new ParcoursNotFoundException(idParcours.ToString());
        
        // Vérifier que l'UE n'est pas déjà associée au parcours
        // Ici, nous vérifions depuis le côté parcours (on peut aussi vérifier depuis le côté UE si nécessaire)
        Parcours parcours = parcoursList.First();
        bool duplicate = parcours.UesEnseignees != null && parcours.UesEnseignees.Any(u => u.Id == idUe);
        if (duplicate)
            throw new DuplicateUeDansParcoursException($"{idUe} est déjà inscrit dans le parcours : {idParcours}");
    }
}