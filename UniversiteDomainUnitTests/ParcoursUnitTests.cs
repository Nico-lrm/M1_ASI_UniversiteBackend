using System.Linq.Expressions;
using Moq;
using UniversiteDomain.Adapters;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Add;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;

namespace UniversiteDomainUnitTests;

public class ParcoursUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long id = 1;
        String nomParcours = "Ue 1";
        int anneFormation = 2;
        
        // On crée l'étudiant qui doit être ajouté en base
        Parcours parcoursSansId = new Parcours{NomParcours= nomParcours, Annee = anneFormation};
        
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mock = new Mock<IRepositoryFactory>();
        
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition    
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Parcours>();
        
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.ParcoursRepository().FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Parcours parcoursCree =new Parcours{Id=id,NomParcours= nomParcours, Annee = anneFormation};
        mock.Setup(repoParcours=>repoParcours.ParcoursRepository().CreateAsync(parcoursSansId)).ReturnsAsync(parcoursCree);
        
        // On crée le bouchon (un faux repositoryFactory). Il est prêt à être utilisé
        var fauxParcoursRepository = mock.Object;
        
        // Création du use case en injectant notre faux repository
        CreateParcoursUseCase useCase=new CreateParcoursUseCase(fauxParcoursRepository);
        
        // Appel du use case
        var parcoursTeste=await useCase.ExecuteAsync(parcoursSansId);
        
        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursCree.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursCree.NomParcours));
        Assert.That(parcoursTeste.Annee, Is.EqualTo(parcoursCree.Annee));
    }
    
    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", Annee = 1};
        
        // On initialise des faux repositories
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(new Etudiant{Id=1});
        mockEtudiant.Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant))).ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = parcours;
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours.Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours))).ReturnsAsync(parcourses);
        mockParcours.Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant)).ReturnsAsync(parcoursFinal);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcoursUseCase useCase=new AddEtudiantDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idEtudiant);
        
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
    
    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        long idUe = 1;
        long idParcours = 3;
        Ue ue = new Ue { Id = 1, NumeroUe = "1", Intitule = "MonUE" };
        Parcours parcours = new Parcours{ Id = 3, NomParcours = "Ue 3", Annee = 1 };
        
        // On initialise des faux repositories
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        
        List<Ue> ues = new List<Ue>();
        ues.Add(new Ue{Id=1});
        mockUe.Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idUe))).ReturnsAsync(ues);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        Parcours parcoursFinal = parcours;
        parcoursFinal.UesEnseignees.Add(ue);
        
        mockParcours.Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours))).ReturnsAsync(parcourses);
        mockParcours.Setup(repo => repo.AddUeAsync(idParcours, idUe)).ReturnsAsync(parcoursFinal);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddUeDansParcoursUseCase useCase=new AddUeDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idUe);
        
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Empty);
        Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    }
}
