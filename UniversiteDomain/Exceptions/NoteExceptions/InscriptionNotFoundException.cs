namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InscriptionNotFoundException : Exception
{
    public InscriptionNotFoundException() : base() { }
    public InscriptionNotFoundException(string message) : base(message) { }
    public InscriptionNotFoundException(string message, Exception inner) : base(message, inner) { }
}