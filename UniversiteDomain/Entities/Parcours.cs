namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = String.Empty;
    public int Annee { get; set; }
    
    // OneToMany : un parcours contient plusieurs étudiants
    public List<Etudiant>? Inscrits { get; set; } = new List<Etudiant>();
    // ManyToMany : un parcours contient plusieurs Ues  
    public List<Ue>? UesEnseignees { get; set; } = new List<Ue>();

    public override string ToString()
    {
        return "ID "+Id +" : "+NomParcours+" - Master "+Annee;
    }
}