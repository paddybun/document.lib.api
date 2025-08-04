namespace document.lib.core;

public interface IUnitOfWork<out TConnection>: IUnitOfWork
{
    TConnection Connection { get; }
}

public interface IUnitOfWork: IDisposable
{
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}