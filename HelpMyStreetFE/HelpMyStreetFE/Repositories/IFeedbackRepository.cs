using System;
using HelpMyStreetFE.Models.Feedback;
using System.Threading.Tasks;
using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Contracts;

namespace HelpMyStreetFE.Repositories
{
    public interface IFeedbackRepository
    {
        Task<List<Testimonial>> GetTestimonials();
        Task<bool> GetFeedbackExists(int jobId, RequestRoles requestRole, int? userId);
        Task<bool> PostRecordFeedback(int jobId, RequestRoles requestRoles, int? userId, FeedbackRating feedbackRating);
        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
    }
}
