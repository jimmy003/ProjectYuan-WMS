using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoCompleteTextBox.Editors;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.Providers
{
    public class ProductSuggestionProvider : ISuggestionProvider
    {

        public IEnumerable<ProductDisplayModel> Products { get; set; } = new List<ProductDisplayModel>();

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                Products
                    .Where(state => state.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

        }



    }
}
