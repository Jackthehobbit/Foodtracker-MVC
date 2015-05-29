using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jack_FoodTrackerMVC.Models;
using Jack.FoodTracker;
using Jack.FoodTracker.Entities;
using System.ComponentModel.DataAnnotations;

namespace Jack_FoodTrackerMVC.Controllers
{
    public class FoodsController : Controller
    {
        private readonly FoodTracker _ftracker;
        public FoodsController(FoodTracker ftracker )
        {
            _ftracker = ftracker;
        }
        // GET: Foods
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Search(FoodDTO searchCriteria)
        {
            IList<Food> Model = _ftracker.GetFoodByExpression(searchCriteria);
            return PartialView("FoodTable", Model);
        }

        public ActionResult GetAll()
        {
            IList<Food> Model = _ftracker.GetAllFood();
            return PartialView("FoodTable", Model);
        }

        // GET: Foods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int nonNullId = (int)id;
            Food food = _ftracker.GetFoodByID(nonNullId);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // GET: Foods/Create
        public ActionResult Create()
        {
            FoodView model = new FoodView(_ftracker.GetAllFoodCategories(false));
            return View(model);
        }

        // POST: Foods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,CategoryName,Calories,Sugars,Fat,Saturates,Salt")] FoodView foodview)
        {
            FoodCategory cat = _ftracker.GetCategoryByName(foodview.CategoryName);
            if (ModelState.IsValid && cat != null)
            {
                try
                {
                    _ftracker.AddFood(foodview, cat);
                    return RedirectToAction("Index");
                }
                catch(ValidationException vexp)
                {
                    
                    foodview.categories = new SelectList(_ftracker.GetAllFoodCategories(false),"Name","Name");
                    ModelState.AddModelError("Name", vexp.Message);
                    return View(foodview);
                }
                
            }

            return View(foodview);
        }

        // GET: Foods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int nonNullId = (int)id;
            Food food = _ftracker.GetFoodByID(nonNullId);
            IList<FoodCategory> cats = _ftracker.GetAllFoodCategories(false);
            FoodView foodview = new FoodView(food.Id,food.Name,food.Description,food.Calories,food.Sugars,food.Fat,food.Saturates,food.Salt,food.Category.Name,cats,food.Category);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(foodview);
        }

        // POST: Foods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,CategoryName,Calories,Sugars,Fat,Saturates,Salt,categories")] FoodView foodview)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    _ftracker.EditFood(foodview);
                }
                catch(ValidationException vexp)
                {
                    return View(foodview);
                }
                
                return RedirectToAction("Index");
            }
            return View(foodview);
        }

        // GET: Foods/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int nonNullId = (int)id;
            Food food = _ftracker.GetFoodByID(nonNullId);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // POST: Foods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Food food = _ftracker.GetFoodByID(id);
            _ftracker.DeleteFood(food);
            return RedirectToAction("Index");
        }

       
    }
}
