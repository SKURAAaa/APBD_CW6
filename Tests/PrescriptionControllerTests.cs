using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD_CW6.Controllers;
using APBD_CW6.Data;
using APBD_CW6.Requests;
using Xunit;

public class PrescriptionControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly PrescriptionController _controller;

    public PrescriptionControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new PrescriptionController(_context);
    }

    [Fact]
    public async Task AddPrescription_ReturnsBadRequest_WhenMoreThan10Medicaments()
    {
        var request = new AddPrescriptionRequest
        {
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(1),
            PatientId = 1,
            DoctorId = 1,
            MedicamentIds = Enumerable.Range(1, 11).ToList()
        };

        var result = await _controller.AddPrescription(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddPrescription_ReturnsBadRequest_WhenDueDateIsBeforeDate()
    {
        var request = new AddPrescriptionRequest
        {
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(-1),
            PatientId = 1,
            DoctorId = 1,
            MedicamentIds = new List<int> { 1, 2 }
        };

        var result = await _controller.AddPrescription(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddPrescription_ReturnsNotFound_WhenMedicamentDoesNotExist()
    {
        var request = new AddPrescriptionRequest
        {
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(1),
            PatientId = 1,
            DoctorId = 1,
            MedicamentIds = new List<int> { 1, 2 }
        };

        var result = await _controller.AddPrescription(request);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddPrescription_CreatesNewPatient_WhenPatientDoesNotExist()
    {
        var request = new AddPrescriptionRequest
        {
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(1),
            Patient = new PatientRequest
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now.AddYears(-30),
                Email = "john.doe@example.com"
            },
            DoctorId = 1,
            MedicamentIds = new List<int> { 1, 2 }
        };

        var result = await _controller.AddPrescription(request);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Fact]
    public async Task GetPatientPrescriptions_ReturnsPatientWithPrescriptions()
    {
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Email = "jane.doe@example.com"
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var result = await _controller.GetPatientPrescriptions(patient.Id);

        var actionResult = Assert.IsType<ActionResult<PatientPrescriptionsResponse>>(result);
        var response = Assert.IsType<PatientPrescriptionsResponse>(actionResult.Value);

        Assert.Equal(patient.FirstName, response.Patient.FirstName);
    }
}
