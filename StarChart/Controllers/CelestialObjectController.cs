using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(ApplicationDbContext));
        }
        [HttpGet("{id:int}",Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var CelestialObject = _context.CelestialObjects.FirstOrDefault(a => a.Id == id);
            if(CelestialObject ==null)
            {
                return NotFound();
            }
            CelestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            return Ok(CelestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var CelestialObject = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (CelestialObject.Count==0)
                return NotFound();
            CelestialObject.ForEach(c=>c.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == c.Id).ToList());
            return Ok(CelestialObject);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var CelestialObjects = _context.CelestialObjects.ToList();
            CelestialObjects.ForEach(x => x.Satellites = _context.CelestialObjects.Where(y => y.OrbitedObjectId == x.Id).ToList());
            return Ok(CelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromBody] CelestialObject celestialObject)
        {
            var celestialObjectToSave= _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObjectToSave == null)
                return NotFound();
            celestialObjectToSave.Name = celestialObject.Name;
            celestialObjectToSave.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectToSave.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialObjectToSave);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id,string name)
        {
            var celestialObjectToSave = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObjectToSave == null)
                return NotFound();
            celestialObjectToSave.Name = name;
            _context.CelestialObjects.Update(celestialObjectToSave);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjectToDel = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);
            if (celestialObjectToDel.Count() == 0)
                return NotFound();
            _context.CelestialObjects.RemoveRange(celestialObjectToDel);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
