namespace Backend.Models;

public class Doctor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
}