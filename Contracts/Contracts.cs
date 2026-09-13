namespace Contracts;

public record ClientCreated(int ClientId, string Name, bool IsActive);
public record ClientUpdated(int ClientId, string Name, bool IsActive);
