using System;
using HelpMyStreetFE.Models.Home;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace HelpMyStreetFE.Repositories
{
    public interface IFeedbackRepository
    {
        Task<List<FeedbackMessage>> GetFeedback();
    }
}
