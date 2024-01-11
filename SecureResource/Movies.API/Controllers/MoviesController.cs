﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.API.Data;
using Movies.API.Models;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesContext _context;

        public MoviesController(MoviesContext context)
        {
            _context = context;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie()
        {
            return await _context.Movie.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }
            //smell code test
            var text = "test1";
            if(text == ""){
                text = "";
            }
            
            return movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movie.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieTest(int id, Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            
            return NoContent();
        }

        //[HttpGet("[action]/{selectedFileName}/{selectedTruckModel}/{selectedTravelTimeSettingName}/{selectedCorneringSettingId}/{selectedImportTemplateSettingId}/{selectedPropertiesName}")]
        //public IEnumerable<RPMTravelTimeTest> CalculateTravelTimeFromSegmentFile(string selectedFileName, string selectedTruckModel, string selectedTravelTimeSettingName, string selectedCorneringSettingId, string selectedImportTemplateSettingId,string selectedPropertiesName)
        //{
        //        var travelTimeTestList = new RPMTravelTimeTestCollection();
        //        travelTimeTestList.LoadTravelTimeTestBySegmentFile(_isHaaSProduction,
        //                                                        config.Value.StandardHaulageConnectString,
        //                                                        selectedTruckModel,
        //                                                        selectedTravelTimeSettingName,
        //                                                        selectedCorneringSettingId,
        //                                                        selectedImportTemplateSettingId,
        //                                                        selectedFileName,
        //                                                        selectedPropertiesName);

        //    return travelTimeTestList;
        //}

    }
}
