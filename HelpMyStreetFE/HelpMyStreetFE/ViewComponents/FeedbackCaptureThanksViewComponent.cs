using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class FeedbackCaptureThanksViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("FeedbackCaptureThanksPopup");
        }
    }
}
