using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CypherTools.Core.DataAccess.Repos;
using CypherTools.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CypherTools.Web.Server.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CyphersController : ControllerBase
    {
        private readonly CypherContext _context;

        private readonly ILogger<CyphersController> logger = null;

        public CyphersController(CypherContext context, ILogger<CyphersController> logger)
        {
            _context = context;
            this.logger = logger;
        }

        // GET: api/Cyphers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cypher>>> GetCyphers()
        {
            return await _context.Cyphers.ToListAsync();
        }

        // GET: api/Cyphers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cypher>> Getcypher(int id)
        {
            var cypher = await _context.Cyphers
                .Include(x=>x.Forms)
                .Include(x=>x.EffectOptions)
                .FirstOrDefaultAsync(x=>x.CypherId == id);

            if (cypher == null)
            {
                return NotFound();
            }

            return cypher;
        }

        // PUT: api/Cyphers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> Putcypher(int id, Cypher cypher)
        {
            if (id != cypher.CypherId)
            {
                return BadRequest();
            }

            var existingModel = await _context.Cyphers
                    .Include(x => x.Forms)
                    .Include(x => x.EffectOptions)
                    .FirstOrDefaultAsync(x => x.CypherId == id);

            if (existingModel != null)
            {

                try
                {
                    _context.Entry(existingModel).CurrentValues.SetValues(cypher);

                    //delete form not in saved cypher list
                    foreach (var form in existingModel.Forms)
                    {
                        if (!cypher.Forms.Any(c => c.FormOptionId == form.FormOptionId))
                            _context.CypherFormOptions.Remove(form);
                    }

                    //Update and add form
                    foreach (var form in cypher.Forms)
                    {
                        var existingForm = existingModel.Forms
                            .Where(x => x.FormOptionId == form.FormOptionId)
                            .FirstOrDefault();

                        if (form.FormOptionId == 0)
                        {
                            existingForm = null;
                        }

                        //Update Cypher
                        if (existingForm != null)
                        {
                            _context.Entry(existingForm).CurrentValues.SetValues(form);

                        }
                        //Add Cypher
                        else
                        {
                            existingModel.Forms.Add(form);
                        }
                    }

                    //delete Effect Options not in saved character list
                    foreach (var effectOption in existingModel.EffectOptions)
                    {
                        if (!cypher.EffectOptions.Any(c => c.EffectOptionId == effectOption.EffectOptionId))
                            _context.CypherEffectOptions.Remove(effectOption);
                    }

                    //Update and add Effect Options
                    foreach (var effectOption in cypher.EffectOptions)
                    {
                        var existingForm = existingModel.EffectOptions
                            .Where(x => x.EffectOptionId == effectOption.EffectOptionId)
                            .FirstOrDefault();

                        if (effectOption.EffectOptionId == 0)
                        {
                            existingForm = null;
                        }

                        //Update Cypher
                        if (existingForm != null)
                        {
                            _context.Entry(existingForm).CurrentValues.SetValues(effectOption);

                        }
                        //Add Cypher
                        else
                        {
                            existingModel.EffectOptions.Add(effectOption);
                        }
                    }


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!cypherExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        logger.LogError(ex, ex.Message);
                        return BadRequest();
                    }
                }
            }

            return NoContent();
        }

        // POST: api/Cyphers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Cypher>> Postcypher(Cypher cypher)
        {
            _context.Cyphers.Add(cypher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getcypher", new { id = cypher.CypherId }, cypher);
        }

        // DELETE: api/Cyphers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cypher>> Deletecypher(int id)
        {
            var cypher = await _context.Cyphers.FindAsync(id);
            if (cypher == null)
            {
                return NotFound();
            }

            _context.Cyphers.Remove(cypher);
            await _context.SaveChangesAsync();

            return cypher;
        }

        private bool cypherExists(int id)
        {
            return _context.Cyphers.Any(e => e.CypherId == id);
        }
    }
}
