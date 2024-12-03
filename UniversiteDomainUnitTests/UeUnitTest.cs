using System.Linq.Expressions;
using Moq;
using UniversiteDomain.Adapters;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase()
    {
        long idUe = 1;
        String numUe = "1";
        String titre = "MonUe";
        
        // On crée l'étudiant qui doit être ajouté en base
        Ue ueSansId = new Ue{NumeroUe = numUe, Intitule= titre};
        
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mock = new Mock<IRepositoryFactory>();
        
        // Il faut ensuite aller dans l'use case pour voir quelles fonctions simuler.
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition 
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Ue>();
        
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quel que soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.UeRepository().FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Ue ueCree =new Ue{Id=idUe,NumeroUe = numUe, Intitule= titre};
        mock.Setup(repoUe=>repoUe.UeRepository().CreateAsync(ueSansId)).ReturnsAsync(ueCree);
        
        // On crée le bouchon (un faux repositoryFactory). Il est prêt à être utilisé
        var fauxUeRepository = mock.Object;
        
        // Création du use case en injectant notre faux repository
        CreateUeUseCase useCase=new CreateUeUseCase(fauxUeRepository);
        
        // Appel du use case
        var ueTeste=await useCase.ExecuteAsync(ueSansId);
        
        // Vérification du résultat
        Assert.That(ueTeste.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(ueTeste.Intitule, Is.EqualTo(ueCree.Intitule));
    }
}