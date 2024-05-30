using APBD_CW6.Models;

public class Patient
{
    public int Id { get; set; }  // Zmieniono na 'Id'
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; }
}
