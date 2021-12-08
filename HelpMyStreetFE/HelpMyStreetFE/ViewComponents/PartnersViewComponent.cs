using HelpMyStreetFE.Models;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace HelpMyStreetFE.ViewComponents
{
    public class PartnersViewComponent : ViewComponent
    {
        private IPartnerService _partnerService;

        public PartnersViewComponent(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = new PartnersViewModel
            {
                Partners = await _partnerService.GetPartners()
            };
            return View(viewModel);
        }
    }
}
