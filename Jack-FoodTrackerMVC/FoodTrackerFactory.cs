using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Jack_FoodTrackerMVC.Controllers;
using Jack.FoodTracker;

namespace Jack_FoodTrackerMVC
{
    public class FoodTrackerFactory : DefaultControllerFactory
    {
        private readonly Dictionary<string,Func<RequestContext,IController>> _controllerMap;

        public FoodTrackerFactory(FoodTracker ftracker)
        {
            if (ftracker == null)
            {
                throw new ArgumentNullException("Unit Of Work");
            }
            this._controllerMap = new Dictionary<string, Func<RequestContext, IController>>();
            this._controllerMap["Home"] = context => new HomeController();
            this._controllerMap["Account"] = context => new AccountController();
            this._controllerMap["Foods"] = context => new FoodsController(ftracker);
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            if(this._controllerMap.ContainsKey(controllerName))
            {
                return this._controllerMap[controllerName](requestContext);
            }
            else
            {
                return null;
            }
        }
    }
}