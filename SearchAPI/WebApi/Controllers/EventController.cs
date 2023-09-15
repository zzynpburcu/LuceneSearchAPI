using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.EventOperations.Queries.GetEvents;
using WebApi.Application.EventOperations.Queries.SearchEvent;
using WebApi.DBOperations;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]s")]

    public class EventController : ControllerBase
    {
        private readonly EventDbContext _context;
        private readonly IMapper _mapper;

        public EventController(EventDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult GetEvents([FromQuery] int page, int size)
        {
            GetEventQuery query = new GetEventQuery(_context, _mapper);
            query.Page=page;
            query.Size=size;
            var obj = query.Handle();
            return Ok(obj);

        }
        [HttpGet("search")]
        public ActionResult SearchByField([FromQuery] string columnName, string searchTerm)
        {
            SearchEventQuery query = new SearchEventQuery(_context, _mapper);
            query.ColumnName=columnName;
            query.SearchTerm=searchTerm;
            var obj = query.Handle();
            return Ok(obj);

        }

    }


}