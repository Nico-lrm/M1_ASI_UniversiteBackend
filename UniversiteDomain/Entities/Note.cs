namespace UniversiteDomain.Entities;

public class Note
{
    public long idEtud { get; set; }
    public Etudiant? etudiant { get; set; }
    public long idUe { get; set; }
    public Ue? ue { get; set; }
    public float valeur { get; set; }
}