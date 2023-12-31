﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Model;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int CreateDish(int restaurantId, CreateDishDto dto);
        DishDto GetById(int restaurantId, int dishId);
        List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
        void RemoveByID(int restaurantId, int dishId);
    }
    public class DishService :IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public int CreateDish(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishEntity = _mapper.Map<Dish>(dto);
            dishEntity.RestaurantId = restaurantId;
            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();
            return dishEntity.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);
            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurants = GetRestaurantById(restaurantId);
            var dishesDtos = _mapper.Map<List<DishDto>>(restaurants.Dishes);

            return dishesDtos;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();

        }
        public void RemoveByID(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);
            _context.Remove(dish);
            _context.SaveChanges();

        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context
              .Restaurants
              .Include(r => r.Dishes)
              .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            return restaurant;
        }
        private Dish GetDishById(int restaurantId, int dishId)
        {
            var dish = _context.Dishes.
                FirstOrDefault(d => d.Id == dishId);
            if (dish == null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }
            return dish;
        }
    }
}
