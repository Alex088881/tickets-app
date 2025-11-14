

namespace Tickets.Application.Interfaces
{
    public interface IDataInitializer
    {
        public bool IsInitialized { get; }
        public bool IsInitializing { get; }
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
