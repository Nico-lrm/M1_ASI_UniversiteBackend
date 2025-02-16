using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Data;
 
public class UniversiteDbContext : IdentityDbContext<UniversiteUser, UniversiteRole, string>
{
    public static readonly ILoggerFactory consoleLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });
    
    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }
 
    public UniversiteDbContext():base()
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(consoleLogger)  //on lie le contexte avec le système de journalisation
            .EnableSensitiveDataLogging() 
            .EnableDetailedErrors();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Appeler base une seule fois
        base.OnModelCreating(modelBuilder);

        // --- Configuration de l'entité Etudiant ---
        modelBuilder.Entity<Etudiant>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.ParcoursSuivi)
            .WithMany(p => p.Inscrits)
            .OnDelete(DeleteBehavior.SetNull); // ou DeleteBehavior.Restrict selon vos besoins

        modelBuilder.Entity<Etudiant>()
            .HasMany(e => e.NotesObtenues)
            .WithOne(n => n.etudiant);

        // --- Configuration de l'entité Parcours ---
        modelBuilder.Entity<Parcours>()
            .HasKey(p => p.Id);

        // La relation Many-to-One avec Etudiant est déjà configurée dans Etudiant
        // Relation Many-to-Many avec Ue
        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.UesEnseignees)
            .WithMany(u => u.EnseigneeDans)
            .UsingEntity(j => j.ToTable("ParcoursUe"));  // Spécifie le nom de la table jointe

        // --- Configuration de l'entité Ue ---
        modelBuilder.Entity<Ue>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Ue>()
            .HasMany(u => u.Notes)
            .WithOne(n => n.ue);

        // --- Configuration de l'entité Note ---
        modelBuilder.Entity<Note>()
            .HasKey(n => new { n.idEtud, n.idUe });

        modelBuilder.Entity<Note>()
            .HasOne(n => n.etudiant)
            .WithMany(e => e.NotesObtenues);

        modelBuilder.Entity<Note>()
            .HasOne(n => n.ue)
            .WithMany(u => u.Notes);

        // --- Configuration des entités Identity ---
        modelBuilder.Entity<UniversiteUser>()
            .HasOne<Etudiant>(user => user.Etudiant)
            .WithOne()
            .HasForeignKey<Etudiant>();

        modelBuilder.Entity<Etudiant>()
            .HasOne<UniversiteUser>()
            .WithOne(user => user.Etudiant)
            .HasForeignKey<UniversiteUser>(user => user.EtudiantId);

        modelBuilder.Entity<UniversiteUser>()
            .Navigation(u => u.Etudiant)
            .AutoInclude();
        modelBuilder.Entity<UniversiteRole>();
    }
    
    public DbSet <Parcours>? Parcours { get; set; }
    public DbSet <Etudiant>? Etudiants { get; set; }
    public DbSet <Ue>? Ues { get; set; }
    public DbSet <Note>? Notes { get; set; }
    public DbSet <UniversiteUser>? UniversiteUsers { get; set; }
    public DbSet<UniversiteRole>? UniversiteRoles { get; set; }
    
}