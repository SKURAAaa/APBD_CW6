using APBD_CW6.Models;
using APBD_CW6.Requests;
using Microsoft.AspNetCore.Mvc;
using APBD_CW6.Data;
using APBD_CW6.Models;
using APBD_CW6.Requests;
using Microsoft.EntityFrameworkCore;

namespace APBD_CW6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Prescription>> AddPrescription(AddPrescriptionRequest request)
        {
            if (request.MedicamentIds.Count > 10)
            {
                return BadRequest("Recepta może obejmować maksymalnie 10 leków.");
            }

            if (request.DueDate < request.Date)
            {
                return BadRequest("DueDate musi być większa lub równa Date.");
            }

            var patient = await _context.Patients.FindAsync(request.PatientId);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    DateOfBirth = request.Patient.DateOfBirth,
                    Email = request.Patient.Email
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var medicaments = await _context.Medicaments
                .Where(m => request.MedicamentIds.Contains(m.Id))
                .ToListAsync();

            if (medicaments.Count != request.MedicamentIds.Count)
            {
                return NotFound("Niektóre z leków podanych na recepcie nie istnieją.");
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                PatientId = patient.Id,
                DoctorId = request.DoctorId,
                PrescriptionMedicaments = request.MedicamentIds.Select(id => new PrescriptionMedicament
                {
                    MedicamentId = id,
                    Dose = request.Doses[id]
                }).ToList()
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return prescription;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<PatientPrescriptionsResponse>> GetPatientPrescriptions(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                .ThenInclude(p => p.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                .ThenInclude(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
            {
                return NotFound();
            }

            var response = new PatientPrescriptionsResponse
            {
                Patient = patient,
                Prescriptions = patient.Prescriptions.OrderBy(p => p.DueDate).ToList()
            };

            return response;
        }
    }
}