using Domain.Exceptions;

namespace Domain;

public class Person
{
    public int PersonId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Gender { get; private set; } = null!;
    public int Age { get; private set; }
    public string IdentificationNumber { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;

    protected Person()
    {
    }

    protected Person(string name, string gender, int age, string identificationNumber, string address, string phoneNumber)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ValidationException("El nombre es obligatorio");
        if(age<=0) throw new ValidationException("La edad debe ser mayor a cero");
        if(string.IsNullOrWhiteSpace(identificationNumber)) throw new ValidationException("El número de identificación es obligatorio");
        
        Name=name;
        Gender=gender;
        Age=age;
        IdentificationNumber=identificationNumber;
        Address=address;
        PhoneNumber=phoneNumber;
    }
}