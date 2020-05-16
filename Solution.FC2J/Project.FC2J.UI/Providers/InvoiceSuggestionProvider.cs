using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCompleteTextBox.Editors;
using Project.FC2J.Models.Sale;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.Providers
{
    public class InvoiceSuggestionProvider : ISuggestionProvider
    {

        private readonly IEnumerable<Invoice> _invoices ;

        public InvoiceSuggestionProvider(IEnumerable<Invoice> invoices)
        {
            _invoices = invoices;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                _invoices
                    .Where(state => state.PoNo.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

        }
    }
}
