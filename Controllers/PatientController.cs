using Microsoft.AspNetCore.Mvc;
using APBD_CW6.Data;
using APBD_CW6.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using APBD_CW6.Requests;

namespace APBD_CW6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientDetails(int id)
        {
            // Pobranie pacjenta wraz z receptami, lekami i lekarzami
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                .ThenInclude(p => p.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                .ThenInclude(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            // Przygotowanie odpowiedzi z danymi pacjenta i jego receptami
            var response = new PatientPrescriptionsResponse
            {
                Patient = patient,
                Prescriptions = patient.Prescriptions.OrderBy(p => p.DueDate).ToList()
            };

            return Ok(response);
        }
    }
}