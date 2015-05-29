﻿using Jack.FoodTracker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Jack.FoodTracker.EntityDatabase
{
    public class FoodRepository : IFoodRepository
    {
        private readonly FoodContext context;

        public FoodRepository(FoodContext context)
        {
            this.context = context;
        }

        public void Add(Food newFood)
        {
            context.Foods.Add(newFood);
        }

        public void Delete(Food food)
        {
            context.Foods.Remove(food);
        }

        public void Edit(Food changedFood)
        {
            context.Entry(changedFood).State = System.Data.Entity.EntityState.Modified;
        }

        public IList<Food> GetAll()
        {
            return context.Foods.ToList();
        }

        public IList<Food> GetByCategory(FoodCategory fCat)
        {
            return context.Foods.Where(x => x.Category.Id == fCat.Id).ToList();
        }

        public IList<Food> SearchByName(String searchText)
        {
            return context.Foods.Where(x => x.Name.Contains(searchText)).ToList();
        }

        public Food GetById(int id)
        {
            return context.Foods.Where(x => x.Id == id).FirstOrDefault();
        }

        public IList<Food> GetByExpression(Expression<Func<Food, Boolean>> lambda)
        {
            return context.Foods.AsQueryable().Where(lambda).ToList();
        }
      
    }
}
