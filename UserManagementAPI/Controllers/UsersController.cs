using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserRepository repo) : ControllerBase
    {

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get(CancellationToken cancellationToken)
        {
            var users = await repo.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> Get(
            Guid id,
            CancellationToken ct)
        {
            try
            {
                var user = await repo.GetByIdAsync(id, ct);
                if (user is null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }

        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> Post(User user, CancellationToken cancellationToken)
        {
            await repo.CreateAsync(user, cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = user.Id },
                user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(
            Guid id,
            User updated,
            CancellationToken cancellationToken)
        {
            var existing = await repo.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();

            updated.Id = id;
            await repo.UpdateAsync(updated, cancellationToken);
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var existing = await repo.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();

            await repo.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}