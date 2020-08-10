using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace HelpMyStreetFE.ViewComponents
{
    public class FeedbackViewComponent : ViewComponent
    {
        private IFeedbackRepository _feedbackRepository;

        public FeedbackViewComponent(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(FeedbackMessageType feedbackMessageType, int? numberToShow)
        {
            var viewModel = new FeedbackViewModel();
            var messages = await _feedbackRepository.GetFeedback();
            if (feedbackMessageType != FeedbackMessageType.Other)
            {
                messages = messages.FindAll(e => e.Type == feedbackMessageType);
            }
            else
            {
                var faceMasks = messages.FindAll(e => e.Type == FeedbackMessageType.FaceCovering);

                var notMasks = messages.FindAll(e => e.Type != FeedbackMessageType.FaceCovering);
                messages = notMasks.Concat(faceMasks.OrderBy(x => Guid.NewGuid()).Take(3)).ToList(); //get fewer masks involved
            }

            if (numberToShow != null)
            {
                messages = messages.OrderBy(x => Guid.NewGuid()).Take(numberToShow.Value).ToList();
            }

            viewModel.FeedbackMessages = messages;
            return View(viewModel);
        }
    }
}
