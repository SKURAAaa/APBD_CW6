using System;
using System.Collections.Generic;

namespace APBD_CW6.Requests
{
    public class AddPrescriptionRequest
    {
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public List<int> MedicamentIds { get; set; }
        public Dictionary<int, int> Doses { get; set; } // klucz to ID leku, wartość to dawka
        public PatientRequest Patient { get; set; } // Jeśli nowy pacjent
    }

    public class PatientRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
    }
}