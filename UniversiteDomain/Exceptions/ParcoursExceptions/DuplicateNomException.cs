namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class DuplicateNomException : Exception
{
    public DuplicateNomException() : base() { }
    public DuplicateNomException(string message) : base(message) { }
    public DuplicateNomException(string message, Exception inner) : base(message, inner) { }
}