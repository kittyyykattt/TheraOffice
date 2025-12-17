using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheraOffice.Api.Models;
using TheraOffice.Api.Repositories;

namespace TheraOffice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Patient> Create([FromBody] Patient patient)
        {
            PatientsRepository.Add(patient);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAll()
        {
            return Ok(PatientsRepository.GetAll());
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Patient> GetById(Guid id)
        {
            var patient = PatientsRepository.GetById(id);
            if (patient is null) return NotFound();
            return Ok(patient);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] Patient patient)
        {
            var success = PatientsRepository.Update(id, patient);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var success = PatientsRepository.Delete(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Patient>> Search([FromQuery] string query)
        {
            var results = PatientsRepository.Search(query ?? string.Empty);
            return Ok(results);
        }
    }
}

