using System.Collections.Generic;

namespace James.Web.ViewModels.Workflows
{
    public class IndexViewModel
    {
        public List<DetailsViewModel> Workflows { get; set; }

        public bool Admin { get; set; }
    }
}
