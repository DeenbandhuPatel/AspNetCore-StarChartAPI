using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
