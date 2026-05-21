# Document Management System - Developer Guide

## Project Overview

This is a document management system designed for daily use to quickly scan, upload, and process documents for long-term storage. The system provides an intuitive interface for document handling with metadata management and flexible storage options.

## Architecture

### Technology Stack

- **Backend**: C# / .NET
- **Database**: Entity Framework Core with Code First approach
- **Frontend**: Blazor Server
- **Storage**: Azure Blob Storage and Local SMB Share support
- **Database Providers**: Azure SQL and SQLite

### Architectural Patterns

#### CQRS (Command Query Responsibility Segregation)
The system implements a lightweight CQRS pattern where:
- **Queries**: Read operations that return `Result<T>` - No try-catch blocks (lightweight)
- **Commands**: Write operations that return `Result<T>` - No try-catch blocks (lightweight)
- **UseCases**: Orchestrate Commands and Queries with business logic and error handling

#### UnitOfWork Pattern
All UseCases, Commands, and Queries receive a `UnitOfWork` instance that provides:
- Database context access via `Connection` property
- Transaction management (`BeginTransactionAsync`, `CommitAsync`, `RollbackTransactionAsync`)
- Proper resource disposal

#### Result Pattern
All operations return `Result<T>` with three states:
- **Success**: `Result<T>.Success(value)` - Operation completed successfully
- **Failure**: `Result<T>.Failure(message, exception)` - Operation failed with error
- **Warning**: `Result<T>.Warning(message)` - Operation completed with warnings (e.g., not found)

## Project Structure

### Core Projects

#### document.lib.core
Contains shared core functionality:
- `IUnitOfWork<TConnection>` interface
- `Result<T>` pattern implementation
- `FilteredResult<T>` for paginated results
- Filtering, sorting, and extension helpers
- Shared configuration models

#### document.lib.data.entities
Entity definitions using EF Core conventions:
- `Document`: Core document entity with metadata
- `Folder`: Document organization
- `Register`: Document register/collection
- `RegisterDescription`: Register metadata
- `Category`: Document categorization
- `Tag` and `TagAssignment`: Document tagging
- `BaseFields`: Common audit fields

#### document.lib.data.context
EF Core Database Context:
- `DatabaseContext`: DbContext with all DbSets
- Migrations folder (Code First migrations)
- SavedQueries for complex raw SQL queries

#### document.lib.data.models
Data transfer objects and mapping configurations:
- View models for UI representation
- AutoMapper configurations via `EntityFrameworkMappingConfigurations`
- `MappingModule` for dependency injection

### Business Logic Layer

#### document.lib.bl.contracts
Interfaces for the business logic layer:
- **UseCases**: High-level business operations (e.g., `IDocumentListUseCase<T>`)
- **Queries**: Read operations (e.g., `IDocumentQuery<T>`, `IFoldersQuery<T>`)
- **Commands**: Write operations (e.g., `IUploadBlobCommand`, `IAddToIndexCommand`)
- All interfaces are generic with `where T : IUnitOfWork` constraint

Structure:
```
/Categories
  ICategoriesQuery.cs
  ICategoryQuery.cs
/DocumentHandling
  IGetFolderOverviewUseCase.cs
  IGetRegisterUseCase.cs
  INextDescriptionQuery.cs
/Documents
  /Queries
  /UseCases
/Folders
/RegisterDescriptions
/Tags
/Upload
```

#### document.lib.bl.shared
Concrete implementations of business logic:
- `UnitOfWork` class implementing `IUnitOfWork<DatabaseContext>`
- All Query implementations
- All Command implementations
- All UseCase implementations
- `BuisnessSharedDependencyModule` for DI registration

### Testing

#### document.lib.bl.tests
Integration and unit tests:
- `UnitTestBase`: Base class for tests
- `UnitTestContext`: Test context setup
- Integration tests using real database context
- Test configuration via appsettings.Test.json

### Presentation Layer

#### document.lib.web.v2
Blazor Server application:
- `/Components/Pages`: Blazor page components
- `/Components/Layout`: Layout components
- `/Data`: Data services
- `/Extensions`: Extension methods
- `/Locales`: Localization resources
- `/wwwroot`: Static assets
- Uses Radzen Blazor components for UI

### Database Migration

#### document.lib.ef.startup
Startup project for EF Core migrations:
- Used to generate migrations via `dotnet ef`
- `StartupService` for migration bootstrapping
- Configuration for both Azure SQL and SQLite

## Development Patterns

### Creating a New Query

**1. Define the interface in `document.lib.bl.contracts`:**
```csharp
public interface IMyQuery<in T> where T : IUnitOfWork
{
    Task<Result<MyEntity>> ExecuteAsync(T uow, MyQueryParameters parameters);
}

public record MyQueryParameters(int Id);
```

**2. Implement in `document.lib.bl.shared`:**
```csharp
public class MyQuery(ILogger<MyQuery> logger) : IMyQuery<UnitOfWork>
{
    public async Task<Result<MyEntity>> ExecuteAsync(UnitOfWork uow, MyQueryParameters parameters)
    {
        // No try-catch - lightweight implementation
        logger.LogDebug("Executing MyQuery with Id: {Id}", parameters.Id);
        
        var entity = await uow.Connection.MyEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == parameters.Id);

        if (entity == null)
            return Result<MyEntity>.Warning($"Entity {parameters.Id} not found");

        return Result<MyEntity>.Success(entity);
    }
}
```

### Creating a New Command

**1. Define the interface in `document.lib.bl.contracts`:**
```csharp
public interface IMyCommand<in T> where T : IUnitOfWork
{
    Task<Result<bool>> ExecuteAsync(T uow, MyCommandParameters parameters);
}

public record MyCommandParameters(string Name, string Description);
```

**2. Implement in `document.lib.bl.shared`:**
```csharp
public class MyCommand(ILogger<MyCommand> logger) : IMyCommand<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork uow, MyCommandParameters parameters)
    {
        // No try-catch - lightweight implementation
        logger.LogDebug("Executing MyCommand");
        
        var entity = new MyEntity
        {
            Name = parameters.Name,
            Description = parameters.Description
        };

        uow.Connection.MyEntities.Add(entity);
        // Note: SaveChanges is called by UseCase via CommitAsync()

        return Result<bool>.Success(true);
    }
}
```

### Creating a New UseCase

**1. Define the interface in `document.lib.bl.contracts`:**
```csharp
public interface IMyUseCase<in T> where T : IUnitOfWork
{
    Task<Result<MyViewModel>> ExecuteAsync(T uow, MyUseCaseParameters parameters);
}

public record MyUseCaseParameters(int Id, string Name);
```

**2. Implement in `document.lib.bl.shared`:**
```csharp
public class MyUseCase(
    ILogger<MyUseCase> logger,
    IMyQuery<UnitOfWork> query,
    IMyCommand<UnitOfWork> command) : IMyUseCase<UnitOfWork>
{
    public async Task<Result<MyViewModel>> ExecuteAsync(UnitOfWork uow, MyUseCaseParameters parameters)
    {
        try
        {
            logger.LogInformation("Executing MyUseCase");
            
            // Start transaction if needed
            await uow.BeginTransactionAsync();
            
            // Use queries and commands
            var queryResult = await query.ExecuteAsync(uow, new(parameters.Id));
            if (queryResult.HasError)
                return Result<MyViewModel>.Failure(queryResult.Message);
            
            var commandResult = await command.ExecuteAsync(uow, new(parameters.Name, "Description"));
            if (commandResult.HasError)
            {
                await uow.RollbackTransactionAsync();
                return Result<MyViewModel>.Failure(commandResult.Message);
            }
            
            // Commit transaction
            await uow.CommitAsync();
            
            var viewModel = new MyViewModel { /* map data */ };
            return Result<MyViewModel>.Success(viewModel);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MyUseCase failed");
            await uow.RollbackTransactionAsync();
            return Result<MyViewModel>.Failure(ex.Message, ex);
        }
    }
}
```

**3. Register in DI (in `BuisnessSharedDependencyModule`):**
```csharp
services.AddScoped<IMyQuery<UnitOfWork>, MyQuery>();
services.AddScoped<IMyCommand<UnitOfWork>, MyCommand>();
services.AddScoped<IMyUseCase<UnitOfWork>, MyUseCase>();
```

### Adding a New Entity

**1. Create entity in `document.lib.data.entities`:**
```csharp
public class MyEntity : BaseFields
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}
```

**2. Add DbSet to `DatabaseContext` (in `document.lib.data.context`):**
```csharp
public DbSet<MyEntity> MyEntities { get; set; } = null!;
```

**3. Create and apply migration:**
```bash
# From the solution root
cd document.lib.ef.startup
dotnet ef migrations add AddMyEntity --startup-project . --project ../document.lib.data.context
dotnet ef database update --startup-project .
```

Alternatively, use the PowerShell script:
```powershell
.\efCreateMigration.ps1 -MigrationName AddMyEntity
```

## Storage Architecture

### Current Implementation
- **Azure Blob Storage**: Primary storage for production
- Documents are uploaded to a container named "library-storage"
- Blob path format: `{folder}/{guid}`
- Unsorted documents go to "unsorted" folder

### Planned Implementation
- **Local SMB Share**: Alternative storage for on-premise deployments
- **Storage Abstraction**: Create `IStorageProvider` interface for pluggable storage backends
- **SQLite**: Database option for local deployments

### Storage Extension Points
To add new storage providers:

1. Define `IStorageProvider` interface:
```csharp
public interface IStorageProvider
{
    Task<bool> UploadAsync(string path, Stream content);
    Task<Stream> DownloadAsync(string path);
    Task<bool> DeleteAsync(string path);
    Task<bool> ExistsAsync(string path);
}
```

2. Implement for each provider (Azure, SMB, etc.)

3. Configure storage provider in `appsettings.json`:
```json
{
  "StorageProvider": "Azure|SMB",
  "Azure": {
    "ConnectionString": "..."
  },
  "SMB": {
    "NetworkPath": "\\\\server\\share",
    "Username": "...",
    "Password": "..."
  }
}
```

## Functional Features

### Document Upload Flow
1. User uploads document via Blazor UI (`NewDocument.razor`)
2. File is sent to API endpoint (`/api/upload/single`)
3. `IUploadBlobUseCase` orchestrates:
   - Generate unique blob name (GUID)
   - Upload to storage via `IUploadBlobCommand`
   - Add metadata to database via `IAddToIndexCommand`
   - Commit transaction or rollback on error
4. Document appears in "unsorted" list

### Document Processing Flow
1. User navigates to unsorted documents
2. Opens document details page
3. Fills in metadata:
   - Display name
   - Description
   - Category (from predefined list)
   - Tags (multi-select)
   - Document date
   - Company name
   - Register/Folder assignment
4. Save via `ISaveDocumentUseCase`
5. Document moves from "unsorted" to organized storage

### Document Retrieval
- **List View**: `IDocumentListUseCase` with filtering and pagination
- **Detail View**: `IDocumentQuery` for single document
- **Search**: Filter by category, tags, date range, company
- **Folder View**: `IGetFolderOverviewUseCase` for folder-based navigation

## Configuration

### Database Connection
Configure in `appsettings.json`:
```json
{
  "Config": {
    "DbConnectionString": "Server=...;Database=DocumentLib;..."
  }
}
```

### Azure Storage
Configure in `appsettings.json`:
```json
{
  "Config": {
    "BlobServiceConnectionString": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;"
  }
}
```

### Localization
- Localization files in `document.lib.web.v2/Locales`
- Supports multiple languages for UI strings

## Best Practices

### Error Handling
- ✅ **DO** use try-catch in UseCases
- ❌ **DON'T** use try-catch in Commands and Queries (keep them lightweight)
- ✅ **DO** return `Result<T>` from all operations
- ✅ **DO** log errors with structured logging

### Logging
- Use dependency-injected `ILogger<T>`
- Log at appropriate levels:
  - `LogDebug`: Detailed information with sensitive data
  - `LogInformation`: General flow information
  - `LogWarning`: Abnormal or unexpected events
  - `LogError`: Errors and exceptions

### Transactions
- Use UnitOfWork transaction methods
- Begin transaction in UseCase, not in Commands/Queries
- Always rollback on exceptions
- Commit only after all operations succeed

### Naming Conventions
- **Queries**: `I{EntityName}Query<T>`, `I{EntityName}sQuery<T>` (plural for lists)
- **Commands**: `I{Action}{EntityName}Command<T>` (e.g., `IAddDocumentCommand`)
- **UseCases**: `I{Action}{EntityName}UseCase<T>` (e.g., `IUploadBlobUseCase`)
- **Parameters**: Use records with suffix `Parameters` (e.g., `QueryParameters`)

### Dependency Injection
- Register all business logic in `BuisnessSharedDependencyModule`
- Use appropriate lifetime:
  - Scoped: UseCases, Commands, Queries, DbContext
  - Singleton: Configuration, Loggers
  - Transient: Lightweight stateless services

## Testing Strategy

### Integration Tests
- Use real `DatabaseContext` with test database
- Test full UseCase workflows
- Verify database state changes
- Use `appsettings.Test.json` for test configuration

### Unit Tests
- Mock dependencies (Commands, Queries)
- Test business logic in isolation
- Verify Result<T> states
- Test error handling paths

## Future Enhancements

### Planned Features
1. **OCR Integration**: Extract text from scanned documents
2. **Full-Text Search**: Search document content, not just metadata
3. **Document Versioning**: Track document changes over time
4. **Workflow**: Approval workflows for document processing
5. **API**: RESTful API for external integrations
6. **Mobile App**: Mobile scanning and upload
7. **Email Integration**: Import documents from email attachments
8. **Audit Trail**: Complete history of document changes
9. **Advanced Security**: Role-based access control, encryption at rest

### Architecture Improvements
1. **Storage Abstraction Layer**: Pluggable storage providers
2. **Message Queue**: Asynchronous processing for large uploads
3. **Caching**: Redis cache for frequently accessed metadata
4. **Microservices**: Split into document, storage, and search services
5. **Event Sourcing**: Track all events for audit and replay

## Troubleshooting

### Common Issues

**Migration Errors**
- Ensure `document.lib.ef.startup` is set as startup project
- Verify connection string in appsettings.json
- Check that all entities have DbSet in DatabaseContext

**Upload Failures**
- Verify Azure Storage connection string
- Check container permissions
- Ensure sufficient storage quota
- Check firewall/network access

**Transaction Errors**
- Ensure BeginTransactionAsync is called before operations
- Always commit or rollback in finally block
- Don't nest transactions

**DI Resolution Errors**
- Verify all interfaces are registered in DI container
- Check for circular dependencies
- Ensure correct lifetime scopes

## Contributing

When adding new features:
1. Follow the established patterns (CQRS, UnitOfWork, Result<T>)
2. Create interfaces in `document.lib.bl.contracts`
3. Implement in `document.lib.bl.shared`
4. Register in `BuisnessSharedDependencyModule`
5. Write integration tests in `document.lib.bl.tests`
6. Update this documentation

## Resources

### Key Files to Reference
- `document.lib.core/Result.cs` - Result pattern implementation
- `document.lib.core/IUnitOfWork.cs` - UnitOfWork interface
- `document.lib.bl.shared/UnitOfWork.cs` - UnitOfWork implementation
- `document.lib.bl.shared/BuisnessSharedDependencyModule.cs` - DI registration
- `document.lib.data.context/DatabaseContext.cs` - EF Core context
- `document.lib.web.v2/Program.cs` - Application startup and configuration

### External Dependencies
- Entity Framework Core
- Azure.Storage.Blobs
- Radzen.Blazor (UI components)
- Microsoft.Extensions.Logging

---

**Version**: 1.0  
**Last Updated**: May 21, 2026  
**Maintainer**: Development Team

