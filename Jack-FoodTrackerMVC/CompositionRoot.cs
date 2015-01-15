using Jack.FoodTracker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jack.FoodTracker.EntityDatabase;

namespace Jack_FoodTrackerMVC
{
    public class CompositionRoot
    {
        private readonly IControllerFactory _controllerFactory;

        public CompositionRoot()
        {
            this._controllerFactory = CreateControllerFactory();
        }

        public IControllerFactory ControllerFactory 
        {
            get
            {
                return _controllerFactory;
            } 
        }


        private static IControllerFactory CreateControllerFactory()
        {
            //string FoodControllerTypeName = ConfigurationManager.AppSettings["FoodControllerType"];
            //Type FoodControllerType = Type.GetType(FoodControllerTypeName, true);
            //FoodTracker ftracker = Activator.CreateInstance(FoodControllerType) as FoodTracker;
            FoodContext context = new FoodContext();
            IFoodRepository foodrepo = new FoodRepository(context);
            IFoodCategoryRepository fcatrepo = new FoodCategoryRepository(context);
            IPresetMealRepository presetmealrepo = new PresetMealRepository(context);
            UnitOfWork UnitofWork = new UnitOfWork(context,foodrepo,fcatrepo,presetmealrepo);

            FoodTracker ftracker = new FoodTracker(UnitofWork);

            FoodTrackerFactory ftrackerFactory = new FoodTrackerFactory(ftracker);
            return ftrackerFactory;
        }

    }
}