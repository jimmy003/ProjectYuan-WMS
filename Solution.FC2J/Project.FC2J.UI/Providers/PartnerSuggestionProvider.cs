using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoCompleteTextBox.Editors;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.Providers
{
    public class PartnerSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<CustomerDisplayModel> Partners { get; set; } = new List<CustomerDisplayModel>();

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                Partners
                    .Where(state => state.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

        }

    }
}
