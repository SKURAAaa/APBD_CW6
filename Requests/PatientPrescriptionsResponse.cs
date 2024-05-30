using APBD_CW6.Models;

namespace APBD_CW6.Requests;

public class PatientPrescriptionsResponse
{
    public Patient Patient { get; set; }
    public List<Prescription> Prescriptions { get; set; }
}
