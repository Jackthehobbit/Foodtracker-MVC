﻿using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Jack.FoodTracker.Entities
{
    public class FoodView
    {
        [Required]
        [Editable(false)]
        public int id { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Min(0)]
        public int Calories { get; set; }

        [Min(0)]
        public double Sugars { get; set; }

        [Min(0)]
        public double Fat { get; set; }

        [Min(0)]
        public double Saturates { get; set; }

        [Min(0)]
        public double Salt { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public SelectList categories { get; set; }

        public FoodView(IList<FoodCategory> Cats)
        {
            this.categories = new SelectList(Cats, "Name", "Name");
        }
        public FoodView(int id,string name,string desc,int calories,double sugar,double fat,double saturates,double salt,string CategoryName,IList<FoodCategory> cats,FoodCategory selectedCat = null)
        {
            this.id = id;
            this.Name = name;
            this.Description = desc;
            this.Calories = calories;
            this.Sugars = sugar;
            this.Fat = fat;
            this.Saturates = saturates;
            this.Salt = salt;
            this.CategoryName = CategoryName;
            this.categories = new SelectList(cats, "Name", "Name", selectedCat);
        }
        public FoodView()
        {

        }
    }
    public class Food
    {
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Min(0)]
        public int Calories { get; set; }

        [Min(0)]
        public double Sugars { get; set; }

        [Min(0)]
        public double Fat { get; set; }

        [Min(0)]
        public double Saturates { get; set; }

        [Min(0)]
        public double Salt { get; set; }

        [Required]
        public FoodCategory Category { get; set; }

        public List<PresetMeal> PresetMeals { get; set; }

        public Food()
        {
        }

        public Food(string name, FoodCategory category, string description, int calories, double sugars, double fat, double saturates, double salt)
        {
            this.Name = name;
            this.Category = category;
            this.Description = description;
            this.Calories = calories;
            this.Sugars = sugars;
            this.Fat = fat;
            this.Saturates = saturates;
            this.Salt = salt;
        }

        public void Update(Food editedFood)
        {
            Name = editedFood.Name;
            Description = editedFood.Description;
            Category = editedFood.Category;
            Calories = editedFood.Calories;
            Sugars = editedFood.Sugars;
            Fat = editedFood.Fat;
            Saturates = editedFood.Saturates;
            Salt = editedFood.Salt;
        }
    }
}
