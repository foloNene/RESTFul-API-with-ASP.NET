﻿using CourseLibraryAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;

namespace CourseLibraryAPI.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if(mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            //the orderBy string is seperated by "," so we split it.
            var orderByAfterSplit = orderBy.Split(',');

            //apply each orderby clause in reverse order - otherwise, the 
            //IQueryable will be ordered in the wrong order
            foreach(var orderByClause in orderByAfterSplit.Reverse())
            {
                //trim the orderBY clause as it might contain leading
                // or trailing spaces. can't trim the var in foreach,
                // so use another var
                var trimmedOrderByClause = orderByClause.Trim();

                //if the sort option ends with "desc", we order
                // descending, otherwise ascending
                var orderDescending = trimmedOrderByClause.EndsWith("desc");

                //remove "asc" or "desc" from the orderByClause.IndexOf(" ");
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                //get the PropertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];

                if(propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                //Run through the property names in reverse
                // so the orderby clause are applied in the correct order
                foreach(var destinationProperty in
                    propertyMappingValue.DestinationProperties.Reverse())
                {
                    // revert sort order if neccessary
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    source = source.OrderBy(destinationProperty +
                        (orderDescending ? " descending" : "ascending"));
                }

            }
            return source;
        }

    }
}
