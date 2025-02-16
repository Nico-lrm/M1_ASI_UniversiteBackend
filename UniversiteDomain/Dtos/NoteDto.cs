using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteAvecUeDto
{
    public long idEtud { get; set; }
    public long idUe { get; set; }
    public UeDto UeDto{get; set;}
    public float valeur { get; set; }

    public NoteAvecUeDto ToDto(Note note)
    {
        idEtud = note.idEtud;
        idUe = note.idUe;
        UeDto = new UeDto().ToDto(note.ue);
        valeur = note.valeur;
        return this;
    }
    
    public Note ToEntity()
    {
        return new Note {idEtud = this.idEtud, idUe = this.idUe, valeur = this.valeur};
    }
    
    public static List<NoteAvecUeDto> ToDtos(List<Note> notes)
    {
        List<NoteAvecUeDto> dtos = new();
        foreach (var note in notes)
        {
            dtos.Add(new NoteAvecUeDto().ToDto(note));
        }
        return dtos;
    }

    public static List<Note> ToEntities(List<NoteAvecUeDto> noteDtos)
    {
        List<Note> notes = new();
        foreach (var noteDto in noteDtos)
        {
            notes.Add(noteDto.ToEntity());
        }

        return notes;
    }
}