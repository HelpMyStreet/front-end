using HelpMyStreetFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.ViewComponents
{
    public class HeaderLoginViewComponent : ViewComponent
    {
        
        private readonly IConfiguration _configuration;

        public HeaderLoginViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            LoginViewModel model = new LoginViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            var routeValues = Request.RouteValues;
            if (ViewData["SignUpGroupId"] != null)
            {
                model.SignUpURL = $"{model.SignUpURL}/{ViewData["SignUpGroupId"]}";
            }
            return View(model);
        }
   
    }
}
