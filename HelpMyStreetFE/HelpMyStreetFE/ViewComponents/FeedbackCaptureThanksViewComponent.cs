using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Feedback;
using Microsoft.AspNetCore.Mvc;

namespace HelpMyStreetFE.ViewComponents
{
    public class FeedbackCaptureThanksViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(CapturedFeedback capturedFeedback)
        {
            return View("FeedbackCaptureThanksPopup", capturedFeedback);
        }
    }
}
