using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Model;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpPost]
        public ActionResult Post([FromRoute]int restaurantId, [FromBody] CreateDishDto createDishDto) 
        {
            var newDishId = _dishService.CreateDish(restaurantId, createDishDto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = _dishService.GetById(restaurantId, dishId);

            return Ok(dish);
        }

        [HttpGet()]
        public ActionResult<List<DishDto>> Get([FromRoute] int restaurantId)
        {
            var dishes = _dishService.GetAll(restaurantId);

            return Ok(dishes);
        }

        [HttpDelete]
        public ActionResult Delete([FromRoute] int restaurantId)
        {
            _dishService.RemoveAll(restaurantId);
            return NoContent();
        }
        [HttpDelete("{dishId}")]
        public ActionResult DeleeteById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.RemoveByID(restaurantId, dishId);
            return NoContent();
        }
    }
}

