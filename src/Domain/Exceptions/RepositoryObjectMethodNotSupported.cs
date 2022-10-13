namespace Domain.Exceptions
{
    public class RepositoryObjectMethodNotSupported : Exception
    {
        public RepositoryObjectMethodNotSupported()
        {
        }

        public RepositoryObjectMethodNotSupported(string message)
            : base(message)
        {
        }

        public RepositoryObjectMethodNotSupported(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}