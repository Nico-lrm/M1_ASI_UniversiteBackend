using UniversiteDomain.Adapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
  // Ajoute un étudiant dans le parcours
        public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
        {
            parcours.Inscrits.Add(etudiant);
            await Context.SaveChangesAsync();
            return parcours;
        }
    
        public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
            if (parcours == null || etudiant == null)
                throw new Exception("Parcours ou Etudiant non trouvé.");
            
            parcours.Inscrits.Add(etudiant);
            await Context.SaveChangesAsync();
            return parcours;
        }
    
        public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
        {
            if (parcours == null)
                throw new ArgumentNullException(nameof(parcours));
            foreach (var etudiant in etudiants)
            {
                parcours.Inscrits.Add(etudiant);
            }
            await Context.SaveChangesAsync();
            return parcours;
        }
    
        public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            if (parcours == null)
                throw new Exception("Parcours non trouvé.");
            foreach (var idEtudiant in idEtudiants)
            {
                var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
                if (etudiant != null)
                {
                    parcours.Inscrits.Add(etudiant);
                }
            }
            await Context.SaveChangesAsync();
            return parcours;
        }
    
        // Ajoute une unité d'enseignement (UE) dans le parcours
        public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
        {
            // On suppose que l'entité Parcours dispose d'une collection 'Ues'
            parcours.UesEnseignees.Add(ue);
            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            var ue = await Context.Ues.FindAsync(idUe);
            if (parcours == null || ue == null)
                throw new Exception("Parcours ou UE non trouvé(e).");

            parcours.UesEnseignees.Add(ue);
            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
        {
            if (parcours == null)
                throw new ArgumentNullException(nameof(parcours));
            foreach (var ue in ues)
            {
                parcours.UesEnseignees.Add(ue);
            }
            await Context.SaveChangesAsync();
            return parcours;
        }
    
        public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            if (parcours == null)
                throw new Exception("Parcours non trouvé.");
            foreach (var idUe in idUes)
            {
                var ue = await Context.Ues.FindAsync(idUe);
                if (ue != null)
                {
                    parcours.UesEnseignees.Add(ue);
                }
            }
            await Context.SaveChangesAsync();
            return parcours;
        }
}