namespace APBD_CW6.Models
{
    public class PrescriptionMedicament
    {
        // Identyfikator recepty
        public int PrescriptionId { get; set; }

        // Powiązana recepta
        public Prescription Prescription { get; set; }

        // Identyfikator leku
        public int MedicamentId { get; set; }

        // Powiązany lek
        public Medicament Medicament { get; set; }

        // Dawka leku
        public int Dose { get; set; }
    }
}