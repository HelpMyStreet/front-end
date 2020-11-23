using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Volunteers
{
    public class VolunteerListViewModel
    {
        public int GroupId { get; set; }
        public IEnumerable<GroupCredential> GroupCredentials { get; set; }
        public IEnumerable<VolunteerViewModel> Volunteers { get; set; }
        public int UserId { get; set; }
    }
}
