﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechLibrary.Data;
using TechLibrary.Domain;
using TechLibrary.Models;

namespace TechLibrary.Helpers
{
    public static class FilterHelpers
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> queryable, string filterType, string filterString)
        {
            if (!string.IsNullOrWhiteSpace(filterString) && !string.IsNullOrWhiteSpace(filterType))
            {
                switch (filterType)
                {
                    case "Title":
                        queryable = queryable
                             // make case insensitive with ToLower()
                             .Where(p => p.Title.ToLower().Contains(filterString.ToLower()));
                        break;
                    case "Description":
                        queryable = queryable
                            // make case insensitive with ToLower()
                            .Where(p => p.ShortDescr.ToLower().Contains(filterString.ToLower()));
                        break;
                }
            }
            return queryable;
        }
    }
}
