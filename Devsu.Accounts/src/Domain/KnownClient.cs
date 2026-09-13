namespace Domain;

public class KnownClient
{
    public int ClientId { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    
    protected KnownClient()
    {
    }
    public KnownClient(int clientId, string name, bool isActive)
    {
        ClientId = clientId;
        Name = name;
        IsActive = isActive;
    }

    public void UpdateFrom(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
    }
}