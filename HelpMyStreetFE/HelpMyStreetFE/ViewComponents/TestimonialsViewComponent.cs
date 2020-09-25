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
    public class TestimonialsViewComponent : ViewComponent
    {
        private IFeedbackRepository _feedbackRepository;

        public TestimonialsViewComponent(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(FeedbackMessageType feedbackMessageType, int? numberToShow, string groupKey, bool? b2bFeedback)
        {
            var viewModel = new FeedbackViewModel();
            var messages = await _feedbackRepository.GetFeedback();
            if (feedbackMessageType == FeedbackMessageType.Other)
            {
                var faceMasks = messages.FindAll(e => e.Type == FeedbackMessageType.FaceCovering);

                var notMasks = messages.FindAll(e => e.Type != FeedbackMessageType.FaceCovering);
                messages = notMasks.Concat(faceMasks.OrderBy(x => Guid.NewGuid()).Take(3)).ToList(); //get fewer masks involved
            }
            else
            {
                messages = messages.FindAll(e => e.Type == feedbackMessageType);

                if (feedbackMessageType == FeedbackMessageType.Group)
                {
                    messages = messages.FindAll(e => e.GroupKey == groupKey);
                }
            }

            if (b2bFeedback != null)
            {
                messages = messages.Where(m => m.B2BFeedback == b2bFeedback).ToList();
            }

            if (numberToShow != null)
            {
                messages = messages.OrderBy(x => Guid.NewGuid()).Take(numberToShow.Value).ToList();
            }

            viewModel.FeedbackMessages = messages.OrderBy(x => Guid.NewGuid()).ToList();
            return View(viewModel);
        }
    }
}
