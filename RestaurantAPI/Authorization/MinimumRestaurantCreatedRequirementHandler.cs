using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Authorization
{
    public class MinimumRestaurantCreatedRequirementHandler : AuthorizationHandler<MinimumRestaurantCreatedRequirement>
    {
        private readonly RestaurantDbContext _dbContext;

        public MinimumRestaurantCreatedRequirementHandler(RestaurantDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumRestaurantCreatedRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var restaurantCreaed = _dbContext.Restaurants.Count(r => r.CreatedById == userId);
            if (restaurantCreaed >= requirement.MinimumRestaurantCreated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
