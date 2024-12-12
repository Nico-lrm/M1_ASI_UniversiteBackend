using System.Linq.Expressions;
using Moq;
using UniversiteDomain.Adapters;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Add;
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
        // Données nécessaires pour effectuer le test
        long idEtudiant = 1;
        long idUe = 2;
        long idParcours = 3;
        float value = 10.0f;
        Etudiant etudiant = new Etudiant{Id=idEtudiant, NumEtud=idEtudiant.ToString(), Nom = "DURAND", Prenom="Jean", Email="jean.durand@hotmail.fr"};
        Ue ue = new Ue { Id = idUe, NumeroUe = idUe.ToString(), Intitule = "MonUE" };
        Parcours parcours = new Parcours{ Id = idParcours, NomParcours = "Ue 3", Annee = 1 };
        
        // On initialise des faux repositories
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockNote = new Mock<INoteRepository>();
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(facto=>facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto=>facto.NoteRepository()).Returns(mockNote.Object);
        
        // On effectue les différentes connexions entre nos entités
        parcours.UesEnseignees.Add(ue);
        parcours.Inscrits.Add(etudiant);
        etudiant.ParcoursSuivi = parcours;
        ue.EnseigneeDans.Add(parcours);
        
        // Préparer les retours des mockup
        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        mockParcours.Setup(repo => repo.FindByConditionAsync(p => p.Inscrits.Find(inscrit => inscrit.Id.Equals(idEtudiant)) != null && p.UesEnseignees.Find(ueEns => ueEns.Id.Equals(idUe)) != null)).ReturnsAsync(parcourses);
        
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(etudiant);
        mockEtudiant.Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idEtudiant))).ReturnsAsync(etudiants);
        
        List<Ue> ues = new List<Ue>();
        ues.Add(ue);
        mockUe.Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idUe))).ReturnsAsync(ues);
        
        List<Note> notes = new List<Note>();
        Note note = new Note{ etudiant = etudiant, idEtud = idEtudiant, idUe = idUe, ue = ue, valeur = value };
        mockNote.Setup(repo => repo.FindByConditionAsync(n => n.idUe.Equals(idUe) && n.idEtud.Equals(idEtudiant))).ReturnsAsync(notes);
        mockNote.Setup(repo => repo.AddNoteAsync(idUe, idEtudiant, value)).ReturnsAsync(note);
        
        // Tester si le use case
        AddNoteEtudiantUeUseCase useCase = new AddNoteEtudiantUeUseCase(mockFactory.Object);
        var result = await useCase.ExecuteAsync(ue, etudiant, value);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result.idEtud, Is.EqualTo(note.idEtud));
        Assert.That(result.etudiant, Is.Not.Null);
        Assert.That(result.etudiant, Is.EqualTo(note.etudiant));
        Assert.That(result.idUe, Is.EqualTo(note.idUe));
        Assert.That(result.ue, Is.Not.Null);
        Assert.That(result.ue, Is.EqualTo(note.ue));
        Assert.That(result.valeur, Is.EqualTo(note.valeur));
    }
}
