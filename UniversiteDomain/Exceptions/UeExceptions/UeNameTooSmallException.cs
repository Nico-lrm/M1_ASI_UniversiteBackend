namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class UeNameTooSmallException : Exception
{
    public UeNameTooSmallException() : base() { }
    public UeNameTooSmallException(string message) : base(message) { }
    public UeNameTooSmallException(string message, Exception inner) : base(message, inner) { }
}