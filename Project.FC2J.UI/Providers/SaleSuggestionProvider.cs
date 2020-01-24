using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoCompleteTextBox.Editors;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.Providers
{
    public class SaleSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<SalesDisplayModel> Sales { get; set; } = new List<SalesDisplayModel>();

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                Sales
                    .Where(state => state.PONo.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

        }

    }
}
