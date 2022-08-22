using DemoWebApi.Models;
using DemoWebApi.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeptController : ControllerBase
    {
        DB1045Context db = new DB1045Context();
        [HttpGet]
        [Route("ListDept")]
        public IActionResult GetDept()//by naming convetion we have to write get
        {
           // var data = db.Depts.ToList();
           // var data = from dept in db.Depts select dept;
           var data = from dept in db.Depts select new {Id = dept.Id, Name = dept.Name, Location = dept.Location};// to select specific columns
           return Ok(data);
            
        }
        [HttpGet]
        [Route("ListDept/{id}")]
        public IActionResult GetDept(int? id)//by naming convetion we have to write get
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null");
            }
            var data = db.Depts.Where(d => d.Id == id).Select(d => new { id = d.Id, Name = d.Name, Location = d.Location });
            if (data == null)
            {
                return NotFound($"Department {id} not present");
            }
            return Ok(data);
        }
        [HttpGet]
        [Route("ListCity")]
        public IActionResult GetCity([FromQuery] string? city)//by naming convetion we have to write get
        {
            var data = db.Depts.Where(d => d.Location == city).Select(d => new { id = d.Id, Name = d.Name, Location = d.Location });
          
            if (data.Count() == 0)//it will take care of error message and not return the empty array.
            {
                return NotFound($"City {city} not present");
            }
            return Ok(data);
        }
        [HttpGet]
        [Route("ShowDept")]
        public IActionResult GetDeptInfo()
        {
            var data = db.DeptInfo_VMs.FromSqlInterpolated<DeptInfo_VM>($"DeptInfo");// by this you can call stored  procedures from database 
            return Ok(data);
        }
        [HttpPost]
        [Route("AddDept")]
        public IActionResult PostDept(Dept dept)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // db.Depts.Add(dept);
                    // db.SaveChanges();
                    //call stored procudre 
                    db.Database.ExecuteSqlInterpolated($"spAddRecordsToDept {dept.Id},{dept.Name},{dept.Location}");// direct adding to database
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            return Created("Record added succesfully", dept);
        }

    }



}
