using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jack.FoodTracker.Entities;
using Jack.FoodTracker.EntityDatabase;

namespace Jack.FoodTracker
{
    public class SearchService
    {
        private readonly Regex comparisonRegex = new Regex("(.*?)(<=|>=|=>|=<|!=|=|<|>)(.*)");
        private readonly Regex comparisonRegexNoKey = new Regex("(<=|>=|=>|=<|!=|=|<|>)(.*)");

        public void ConvertComparison(String searchText, out String key, out String exp, out String val)
        {
            Match matches = comparisonRegex.Match(searchText);
            key = matches.Groups[1].Value;
            exp = matches.Groups[2].Value;
            val = matches.Groups[3].Value;

            if (key == null || exp == null || val == null)
            {
                throw new ArgumentException("Error when searching");
            }
        }

        public void ConvertComparison(String searchText, out String exp, out String val)
        {
            Match matches = comparisonRegex.Match(searchText);
            exp = matches.Groups[1].Value;
            val = matches.Groups[2].Value;
            if (val == null)
            {
                throw new ArgumentException("Error when searching");
            }
        }

        public IList<T> ApplySearchNumerical<T>(IList<T> searchList, string propertyName, string expression, int value)
        {
            return ApplySearchNumerical<T, int>(searchList, propertyName, expression, value);
        }

        public IList<T> ApplySearchNumerical<T>(IList<T> searchList, string propertyName, string expression, double value)
        {
            return ApplySearchNumerical<T, double>(searchList, propertyName, expression, value);
        }

        public IList<T> ApplySearchNumerical<T>(IList<T> searchList, string propertyName, string expression, decimal value)
        {
            return ApplySearchNumerical<T, decimal>(searchList, propertyName, expression, value);
        }

        private IList<T> ApplySearchNumerical<T, TNumeric>(IList<T> searchList, string propertyName, string expression, TNumeric value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression prop = Expression.PropertyOrField(parameter, propertyName);
            Expression body;

            switch (expression)
            {
                case "=":
                    body = Expression.Equal(prop, Expression.Constant(value));
                    break;
                case ">":
                    body = Expression.GreaterThan(prop, Expression.Constant(value));
                    break;
                case "<":
                    body = Expression.LessThan(prop, Expression.Constant(value));
                    break;
                case "<=":
                case "=<":
                    body = Expression.LessThanOrEqual(prop, Expression.Constant(value));
                    break;
                case ">=":
                case "=>":
                    body = Expression.GreaterThanOrEqual(prop, Expression.Constant(value));
                    break;
                case "!=":
                    body = Expression.NotEqual(prop, Expression.Constant(value));
                    break;
                default:
                    throw new ArgumentException(expression + "is not a valid search expression for " + propertyName);
            }

            Expression<Func<T, Boolean>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return searchList.AsQueryable().Where(lambda).ToList();
        }

        public IList<T> ApplySearchEquals<T>(IList<T> searchList, string propertyName, string expression, object value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression prop = Expression.PropertyOrField(parameter, propertyName);
            Expression body;

            if (expression == "!=")
            {
                body = Expression.NotEqual(prop, Expression.Constant(value));
            }
            else
            {
                body = Expression.Equal(prop, Expression.Constant(value));
            }

            Expression<Func<T, Boolean>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return searchList.AsQueryable().Where(lambda).ToList();
        }

        public IList<T> ApplySearchEquals<T>(IList<T> searchList, string propertyName, string expression, string value, bool lowerCase)
        {


            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression prop = Expression.PropertyOrField(parameter, propertyName);

            if (lowerCase)
            {
                value = value.ToLower();

                MethodInfo method = typeof(string).GetMethod("ToLower");
                prop = Expression.Call(prop, method);
            }

            Expression body;

            if (expression == "!=")
            {
                body = Expression.NotEqual(prop, Expression.Constant(value));
            }
            else
            {
                body = Expression.Equal(prop, Expression.Constant(value));
            }

            Expression<Func<T, Boolean>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return searchList.AsQueryable().Where(lambda).ToList();
        }

        public IList<T> ApplySearchContains<T>(IList<T> searchList, string propertyName, string expression, string value, bool lowerCase)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression prop = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            Expression body;

            if (lowerCase)
            {
                value = value.ToLower();

                MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
                prop = Expression.Call(prop, toLowerMethod);
            }

            if (expression == "!=")
            {
                body = Expression.Not(Expression.Call(prop, containsMethod, Expression.Constant(value)));
            }
            else
            {
                body = Expression.Call(prop, containsMethod, Expression.Constant(value));
            }


            Expression<Func<T, Boolean>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return searchList.AsQueryable().Where(lambda).ToList();
        }

        public Expression<Func<Food, Boolean>> BuildExpression(FoodDTO DTO)
        {
            Expression prop;
            ParameterExpression param = Expression.Parameter(typeof(Food), "x");
            Expression body = null;
            string sValue;
            int iValue;
            double dValue;
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            string expression;
            string numValue;
            Expression builtBody; // Used to hold the expression built by buildnumerical for attributes such as fat

            if (DTO.Name != null)
            {
                sValue = DTO.Name.ToLower();
                prop = Expression.PropertyOrField(param, "Name");
                body = Expression.Equal(prop, Expression.Constant(sValue));
            }
            if (DTO.Description != null)
            {
                sValue = DTO.Description.ToLower();
                prop = Expression.PropertyOrField(param, "Description");
                if (body != null)
                {
                    body = Expression.And(body, Expression.Call(prop, containsMethod, Expression.Constant(sValue)));
                }
                else
                {
                    body = Expression.Call(prop, containsMethod, Expression.Constant(sValue));
                }
            }
            if (DTO.Calories != null)
            {
                ConvertComparison(DTO.Calories, out expression, out numValue);
                iValue = Int32.Parse(numValue);
                builtBody = BuildNumerical<Food, Int32>("Calories", expression, iValue);
                if (body != null)
                {
                    body = Expression.And(body, builtBody);
                }
                else
                {
                    body = builtBody;
                }
            }
            if (DTO.Fat != null)
            {
                ConvertComparison(DTO.Fat, out expression, out numValue);
                dValue = Double.Parse(numValue);
                builtBody = BuildNumerical<Food, Double>("Fat", expression, dValue);
                if (body != null)
                {
                    body = Expression.And(body, builtBody);
                }
                else
                {
                    body = builtBody;
                }
            }
            if (DTO.Sugar != null)
            {
                ConvertComparison(DTO.Sugar, out expression, out numValue);
                dValue = Double.Parse(numValue);
                builtBody = BuildNumerical<Food, Double>("Sugars", expression, dValue);
                if (body != null)
                {
                    body = Expression.And(body, builtBody);
                }
                else
                {
                    body = builtBody;
                }
            }
            if (DTO.Saturates != null)
            {
                ConvertComparison(DTO.Saturates, out expression, out numValue);
                dValue = Double.Parse(numValue);
                builtBody = BuildNumerical<Food, Double>("Saturates", expression, dValue);
                if (body != null)
                {
                    body = Expression.And(body, builtBody);
                }
                else
                {
                    body = builtBody;
                }
            }
            if (DTO.Salt != null)
            {
                ConvertComparison(DTO.Salt, out expression, out numValue);
                dValue = Double.Parse(numValue);
                builtBody = BuildNumerical<Food, Double>("Salt", expression, dValue);
                if (body != null)
                {
                    body = Expression.And(body, builtBody);
                }
                else
                {
                    body = builtBody;
                }
            }

            Expression<Func<Food, Boolean>> lambda = Expression.Lambda<Func<Food, bool>>(body, param);

            if (body == null)
            {
                throw new ArgumentException("No Criteria Specified!");
            }
            else
            {
                return lambda;
            }

        }

        private Expression BuildNumerical<T, TNumeric>(string propertyName, string expression, TNumeric value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression prop = Expression.PropertyOrField(parameter, propertyName);
            Expression body;

            switch (expression)
            {
                case "=":
                    body = Expression.Equal(prop, Expression.Constant(value));
                    break;
                case ">":
                    body = Expression.GreaterThan(prop, Expression.Constant(value));
                    break;
                case "<":
                    body = Expression.LessThan(prop, Expression.Constant(value));
                    break;
                case "<=":
                case "=<":
                    body = Expression.LessThanOrEqual(prop, Expression.Constant(value));
                    break;
                case ">=":
                case "=>":
                    body = Expression.GreaterThanOrEqual(prop, Expression.Constant(value));
                    break;
                case "!=":
                    body = Expression.NotEqual(prop, Expression.Constant(value));
                    break;
                default:
                    throw new ArgumentException(expression + "is not a valid search expression for " + propertyName);
            }

            return body;
        }
    }
}

