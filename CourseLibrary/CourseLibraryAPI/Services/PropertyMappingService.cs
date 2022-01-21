using CourseLibraryAPI.Entities;
using CourseLibraryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibraryAPI.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
                {"MainCategor", new PropertyMappingValue(new List<string>(){"MainCategory"}) },
                {"Age", new PropertyMappingValue(new List<string>(){"DateOfBirth"}, true) },
                {"Name", new PropertyMappingValue(new List<string>(){"FirstName","LastName"}) }
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //the string is seperated by " so we split it.
            var fieldAfterSplit = fields.Split(',');

            //run through the fields clauses
            foreach(var field in fieldAfterSplit)
            {
                //trim
                var trimmedField = field.Trim();

                var indexOfFristSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFristSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFristSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            //get matching mapping
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance" +
                $"for <{typeof(TSource)},{typeof(TDestination)}");

        }
    }
}
