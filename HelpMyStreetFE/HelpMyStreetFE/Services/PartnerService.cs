using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
using HMSEnums = HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Services
{
    public class PartnerService : IPartnerService
    {
        public async Task<IEnumerable<Partner>> GetPartners()
        {
            List<Partner> partners = new List<Partner>();
            partners.Add(new Partner() { Group = HMSEnums.Groups.Ruddington, GroupKey = "ruddington"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKWirral, GroupKey = "ageukwirral"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.Southwell, GroupKey = "southwell"});            

            return partners;
        }
    }
}
