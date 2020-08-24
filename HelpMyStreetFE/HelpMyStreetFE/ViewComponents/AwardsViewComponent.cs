using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Awards;
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
    public class AwardsViewComponent : ViewComponent
    {
        private IAwardsRepository _awardsRepository;

        public AwardsViewComponent(IAwardsRepository awardsRepository)
        {
            _awardsRepository = awardsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int awardsLevel)
        {
            var viewModel = new AwardsViewModel();
            var awards = await _awardsRepository.GetAwards();
            awards = awards.OrderBy(x => x.AwardValue).ToList();
            var relevantAwards = awards.Where(x => awardsLevel > x.AwardValue);

            if (relevantAwards.Count() >= 1)
            {
                viewModel.Award = relevantAwards.LastOrDefault();
                viewModel.NextAwardLevel = awards.Where(x => x.AwardValue > awardsLevel).FirstOrDefault().AwardValue;
            }
            else
            {
                viewModel.NextAwardLevel = awards.FirstOrDefault().AwardValue;
            }

            viewModel.CurrentAwardLevel = awardsLevel;
            return View(viewModel);
        }
    }
}
