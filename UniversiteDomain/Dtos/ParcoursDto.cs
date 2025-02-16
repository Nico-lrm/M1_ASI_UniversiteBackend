using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class ParcoursDto
{
    public long Id { get; set; }
    public string NomParcours { get; set; }
    public int Annee { get; set; }

    public ParcoursDto ToDto(Parcours parcours)
    {
        this.Id = parcours.Id;
        this.NomParcours = parcours.NomParcours;
        this.Annee = parcours.Annee;
        return this;
    }
    
    public Parcours ToEntity()
    {
        return new Parcours {Id = this.Id, NomParcours = this.NomParcours, Annee = this.Annee};
    }
}