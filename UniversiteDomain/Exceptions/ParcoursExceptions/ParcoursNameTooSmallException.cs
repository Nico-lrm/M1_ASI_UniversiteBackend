namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class ParcoursNameTooSmallException : Exception
{
    public ParcoursNameTooSmallException() : base() { }
    public ParcoursNameTooSmallException(string message) : base(message) { }
    public ParcoursNameTooSmallException(string message, Exception inner) : base(message, inner) { }
}