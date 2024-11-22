namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteOutOfRangeException : Exception
{
    public NoteOutOfRangeException() : base() { }
    public NoteOutOfRangeException(string message) : base(message) { }
    public NoteOutOfRangeException(string message, Exception inner) : base(message, inner) { }
}