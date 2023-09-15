using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using WebApi.Application.EventOperations.Queries.GetEvents;
using WebApi.Common;
using WebApi.DBOperations;
using WebApi.Entities;

namespace WebApi.Application.EventOperations.Queries.SearchEvent
{
    public class SearchEventQuery
    {
        private readonly EventDbContext _dbContext;
        private readonly IMapper _mapper;
        public string ColumnName { get; set; }
        public string SearchTerm { get; set; }
        public SearchEventQuery(EventDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<EventsViewModel> Handle()
        {

            IQueryable<Event> eventList = null;

            switch (ColumnName.ToLower())
            {
                case "description":
                    eventList = _dbContext.Events.Where(x => x.description.Contains(SearchTerm));
                    break;
                case "lang":
                    eventList = _dbContext.Events.Where(x => x.lang.Contains(SearchTerm));
                    break;
                case "category1":
                    eventList = _dbContext.Events.Where(x => x.category1.Contains(SearchTerm));
                    break;
                case "category2":
                    eventList = _dbContext.Events.Where(x => x.category2.Contains(SearchTerm));
                    break;
                case "date":
                    eventList = _dbContext.Events.Where(x => x.date.Contains(SearchTerm));
                    break;
                    // Diğer sütunlara göre de case'ler ekleyin
            }
            List<EventsViewModel> vm = _mapper.Map<List<EventsViewModel>>(eventList);
            return vm;
        }
    }


}