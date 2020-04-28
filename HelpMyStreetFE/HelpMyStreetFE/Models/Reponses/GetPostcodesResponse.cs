using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Reponses
{
    public class GetPostcodesResponse : BaseResponse
    {
        public HelpMyStreet.Contracts.AddressService.Response.GetPostcodesResponse Content { get; set; }
    }
}
