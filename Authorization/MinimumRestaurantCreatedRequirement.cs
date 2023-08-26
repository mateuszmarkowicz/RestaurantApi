using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumRestaurantCreatedRequirement : IAuthorizationRequirement
    {
        public int MinimumRestaurantCreated { get; }

        public MinimumRestaurantCreatedRequirement(int minimumRestaurantCreated)
        {
            MinimumRestaurantCreated = minimumRestaurantCreated;
        }
    }
}
