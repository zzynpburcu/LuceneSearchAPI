using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using WebApi.Common;
using WebApi.DBOperations;
using WebApi.Entities;

namespace WebApi.Application.EventOperations.Queries.GetEvents
{
    public class GetEventQuery
    {
        private readonly EventDbContext _dbContext;
        private readonly IMapper _mapper;
        public int Page { get; set; }
        public int Size { get; set; }
        public GetEventQuery(EventDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

         public List<EventsViewModel> Handle()
        {
            var eventList = _dbContext.Events.OrderBy(x => x.Id).Skip((Page-1)*Size).Take(Size).ToList<Event>();
            List<EventsViewModel> vm = _mapper.Map<List<EventsViewModel>>(eventList);
            return vm;
        }
    }

    public class EventsViewModel
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Granularity { get; set; }
    }
}