using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Model;
using RestaurantAPI.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        public RestaurantDto GetById(int id);
        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        public int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(UpdateRestaurantDto dto, int id);
    }
    public class RestaurantService: IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger
            , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public RestaurantDto GetById(int id) 
        {
            var rastaurant = _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == id);
            if (rastaurant is null)
            {
                throw new NotFoundException("Restaurant not found")
            }

            var result = _mapper.Map<RestaurantDto>(rastaurant);
            return result;
        }

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                    || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    { nameof(Restaurant.Name), r => r.Name},
                    { nameof(Restaurant.Description), r => r.Description},
                    { nameof(Restaurant.Category), r => r.Category}
                };
                
                var seletionColumn = columnsSelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC ?
                    baseQuery.OrderBy(seletionColumn) 
                    : baseQuery.OrderByDescending(seletionColumn);
            }

            var rastaurants = baseQuery
               .Skip(query.PageSize * (query.PageNumber - 1))
               .Take(query.PageSize)
               .ToList();

            var totalRestaurantCount = baseQuery.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(rastaurants);
            var result = new PagedResult<RestaurantDto>(restaurantsDtos, totalRestaurantCount, query.PageSize, query.PageNumber);
            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }

        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");
            var rastaurant = _dbContext
               .Restaurants
               //.Include(r => r.Address)
               //.Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == id);

            if(rastaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var authorizationResoult = _authorizationService.AuthorizeAsync(_userContextService.User, rastaurant,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResoult.Succeeded)
            {
                throw new ForbidException();
            }

            //_dbContext.Addresses.Remove(rastaurant.Address);
            _dbContext.Restaurants.Remove(rastaurant);
            _dbContext.SaveChanges(true);
        }

        public void Update(UpdateRestaurantDto dto, int id)
        {
            
            var rastaurant = _dbContext
              .Restaurants
              .FirstOrDefault(r => r.Id == id);

            if (rastaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var authorizationResoult = _authorizationService.AuthorizeAsync(_userContextService.User, rastaurant,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if(!authorizationResoult.Succeeded)
            {
                throw new ForbidException();
            }

            rastaurant.Name = dto.Name;
            rastaurant.Description = dto.Description;
            rastaurant.HasDelivery = dto.HasDelivery;
            _dbContext.Update(rastaurant);
            _dbContext.SaveChanges();

        }
    }

   
}
