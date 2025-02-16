namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string NumeroUe { get; set; } = String.Empty;
    public string Intitule { get; set; } = String.Empty;
    
    // ManyToMany : une Ue est enseignée dans plusieurs parcours
    public List<Parcours>? EnseigneeDans { get; set; } = new List<Parcours>();
    public List<Note>? Notes { get; set; } = new List<Note>();
    
    public override string ToString()
    {
        return "ID "+Id +" : "+NumeroUe+" - "+Intitule;
    }
}