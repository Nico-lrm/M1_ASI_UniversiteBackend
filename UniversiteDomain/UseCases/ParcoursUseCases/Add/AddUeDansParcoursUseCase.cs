using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Add;

public class AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    // Rajout d'une Ue dans un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Ue ue)
      {
          ArgumentNullException.ThrowIfNull(parcours);
          ArgumentNullException.ThrowIfNull(ue);
          return await ExecuteAsync(parcours.Id, ue.Id); 
      }  
    public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
      {
          await CheckBusinessRules(idParcours, idUe); 
          return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
      }

    // Rajout de plusieurs étudiants dans un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Ue> ues)
      {
          ArgumentNullException.ThrowIfNull(ues);
          ArgumentNullException.ThrowIfNull(parcours);
          long[] idUes = ues.Select(x => x.Id).ToArray();
          return await ExecuteAsync(parcours.Id, idUes); 
      }  
    public async Task<Parcours> ExecuteAsync(long idParcours, long [] idUes)
      { 
        // Comme demandé par le client, on teste tous les règles avant de modifier les données
        foreach(var id in idUes) await CheckBusinessRules(idParcours, id);
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUes);
      }   

    private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idUe);
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idParcours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche l'ue
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(e=>e.Id.Equals(idUe));;
        if (ue == null) throw new UeNotFoundException(idUe.ToString());
        
        // On recherche le parcours
        List<Parcours> parcours = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p=>p.Id.Equals(idParcours));;
        if (parcours == null) throw new ParcoursNotFoundException(idParcours.ToString());
        
        // On vérifie que l'ue n'est pas déjà dans le parcours
        List<Ue> inscrit = await repositoryFactory.UeRepository().FindByConditionAsync(e=>e.Id.Equals(idUe) && e.EnseigneeDans.Find(p=> p.Id == idParcours) != null);
        if (inscrit is { Count: > 0 }) throw new DuplicateUeDansParcoursException(idUe+" est déjà inscrit dans le parcours dans le parcours : "+idParcours);   
        
    }
}