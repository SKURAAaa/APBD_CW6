using APBD_CW6.Controllers;
using APBD_CW6.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using APBD_CW6.Models;
using Xunit;

namespace APBD_CW6.Tests
{
    public class PatientControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            // Konfiguracja opcji DbContext z użyciem bazy danych w pamięci
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new PatientController(_context);
        }

        [Fact]
        public async Task GetPatientDetails_ShouldReturnOk()
        {
            // Dodanie przykładowego pacjenta do bazy danych
            var patient = new Patient
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = "jan.kowalski@example.com" // Dodano brakującą właściwość
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Dodanie przykładowej recepty dla pacjenta
            var prescription = new Prescription
            {
                Id = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
                PatientId = patient.Id,
                DoctorId = 1,
                Doctor = new Doctor
                {
                    Id = 1,
                    FirstName = "Doctor",
                    LastName = "Who",
                    Email = "doctor.who@example.com"
                }
            };
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            // Wykonanie żądania do kontrolera i sprawdzenie wyniku
            var result = await _controller.GetPatientDetails(1);

            // Sprawdzenie, czy wynik jest typu OkObjectResult
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}