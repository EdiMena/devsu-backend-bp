using Domain.Exceptions;

namespace Domain;

public class Client : Person
{
    public string PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; }

    protected Client()
    {
    }

    public Client(string name, string gender, int age, string identificationNumber, string address, string phoneNumber,string passwordHash)
        : base(name, gender, age, identificationNumber, address, phoneNumber)
    {
        if(string.IsNullOrWhiteSpace(passwordHash)) throw new ValidationException("La contraseña es obligatoria");
        PasswordHash = passwordHash;
        IsActive = true;
    }
    
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void ChangePassword(string newPasswordHash) => PasswordHash = newPasswordHash;
}