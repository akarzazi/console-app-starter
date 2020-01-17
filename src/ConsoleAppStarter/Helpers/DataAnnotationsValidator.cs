using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Validation
{
    using Helpers.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    namespace DataAnnotationsValidator
    {
        // From : https://github.com/reustmd/DataAnnotationsValidatorRecursive
        public static class DataAnnotationsValidator
        {
            public static bool TryValidateObject(object obj, ICollection<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
            {
                return Validator.TryValidateObject(obj, new ValidationContext(obj, null, validationContextItems), results, true);
            }

            public static bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, IDictionary<object, object> validationContextItems = null) where T : notnull
            {
                return TryValidateObjectRecursive(obj, results, new HashSet<object>(), validationContextItems);
            }

            private static bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object> validationContextItems = null) where T : notnull
            {
                //short-circuit to avoid infinit loops on cyclical object graphs
                if (validatedObjects.Contains(obj))
                {
                    return true;
                }

                validatedObjects.Add(obj);
                bool result = TryValidateObject(obj, results, validationContextItems);

                var properties = obj.GetType().GetProperties().Where(prop => prop.CanRead
                    && prop.GetIndexParameters().Length == 0).ToList();

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

                    var value = obj.GetPropertyValue(property.Name);

                    if (value == null) continue;

                    if (value is IEnumerable asEnumerable)
                    {
                        foreach (var enumObj in asEnumerable)
                        {
                            if (enumObj != null)
                            {
                                Validate(enumObj, property, results, validatedObjects, validationContextItems, ref result);
                            }
                        }
                    }
                    else
                    {
                        Validate(value, property, results, validatedObjects, validationContextItems, ref result);
                    }
                }

                return result;
            }

            private static void Validate(object obj, PropertyInfo property, List<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object> validationContextItems, ref bool result)
            {
                var nestedResults = new List<ValidationResult>();
                if (!TryValidateObjectRecursive(obj, nestedResults, validatedObjects, validationContextItems))
                {
                    result = false;
                    foreach (var validationResult in nestedResults)
                    {
                        PropertyInfo property1 = property;
                        results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
                    }
                };
            }
        }
    }
}
