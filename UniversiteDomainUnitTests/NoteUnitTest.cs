using System.Linq.Expressions;
using Moq;
using UniversiteDomain.Adapters;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Add;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;

namespace UniversiteDomainUnitTests;

public class NoteUnitTest
{
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public async Task AddNoteEtudiantUeUseCase()
    {
        long idEtudiant = 1;
        long idUe = 2;
        long idParcours = 3;
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
