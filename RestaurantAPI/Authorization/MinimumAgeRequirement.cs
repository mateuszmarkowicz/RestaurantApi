using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MininumAge { get; }

        public MinimumAgeRequirement(int mininumAge)
        {
            MininumAge = mininumAge;
        }


    }
}
