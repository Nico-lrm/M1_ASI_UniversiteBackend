using Microsoft.AspNetCore.Identity;
using UniversiteDomain.Adapters;
using UniversiteDomain.Adapters.AdapterFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory (UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager): IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private IUniversiteUserRepository? _universiteUsers;
    private IUniversiteRoleRepository? _universiteRoles;
    
    public IParcoursRepository ParcoursRepository()
    {
        if (_parcours == null)
        {
            _parcours = new ParcoursRepository(context ?? throw new InvalidOperationException());
        }
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        if (_etudiants == null)
        {
            _etudiants = new EtudiantRepository(context ?? throw new InvalidOperationException());
        }
        return _etudiants;
    }

    public IUeRepository UeRepository()
    {
        if (_ues == null)
        {
            _ues = new UeRepository(context ?? throw new InvalidOperationException());
        }
        return _ues;
    }

    public INoteRepository NoteRepository()
    {
        if (_notes == null)
        {
            _notes = new NoteRepository(context ?? throw new InvalidOperationException());
        }
        return _notes;

    }

    public IUniversiteUserRepository UniversiteUserRepository()
    {
        if (_universiteUsers == null)
        {
            _universiteUsers = new UniversiteUserRepository(context ?? throw new InvalidOperationException(), userManager, roleManager);
        }
        return _universiteUsers;
    }

    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        if (_universiteRoles == null)
        {
            _universiteRoles = new UniversiteRoleRepository(context ?? throw new InvalidOperationException(), roleManager);
        }
        return _universiteRoles;
    }

    public async Task SaveChangesAsync()
    {
        context.SaveChangesAsync().Wait();
    }
    public async Task EnsureCreatedAsync()
    {
        context.Database.EnsureCreated();
    }
    public async Task EnsureDeletedAsync()
    {
        context.Database.EnsureDeleted();
    }
}