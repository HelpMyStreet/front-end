using System;
using HelpMyStreetFE.Models.Feedback;
using System.Threading.Tasks;
using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Repositories
{
    public interface IFeedbackRepository
    {
        Task<List<Testimonial>> GetTestimonials();
        Task<bool> GetFeedbackExists(int JobId, RequestRoles requestRole);
    }
}
