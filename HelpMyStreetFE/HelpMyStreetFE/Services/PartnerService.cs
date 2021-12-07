using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
using HMSEnums = HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Services
{
    public class PartnerService : IPartnerService
    {

        public PartnerService()
        {

        }

        public async Task<IEnumerable<Partner>> GetPartners()
        {
            List<Partner> partners = new List<Partner>();

            partners.Add(new Partner() { Group = HMSEnums.Groups.FTLOS, GroupKey = "ftlos"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKLSL, GroupKey = "ageuklsl"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.Tankersley, GroupKey = "tankersley"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.Ruddington, GroupKey = "ruddington"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKWirral, GroupKey = "ageukwirral"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKNottsBalderton, GroupKey = "balderton"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKNorthWestKent, GroupKey = "ageuknwkent"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKNottsNorthMuskham, GroupKey = "north-muskham"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKSouthKentCoast, GroupKey = "ageuk-southkentcoast"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.LincolnshireVolunteers, GroupKey = "lincs-volunteers"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKFavershamAndSittingbourne, GroupKey = "ageuk-favershamandsittingbourne"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.EastLindseyPCN, GroupKey = "east-lindsey-pcn"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.LincolnPCN, GroupKey = "apex-pcn-lincoln"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeConnectsCardiff, GroupKey = "ageconnects-cardiff"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.Southwell, GroupKey = "southwell"});
            partners.Add(new Partner() { Group = HMSEnums.Groups.AgeUKMidMersey, GroupKey = "ageuk-midmersey"});


            return partners;
        }
    }
}
