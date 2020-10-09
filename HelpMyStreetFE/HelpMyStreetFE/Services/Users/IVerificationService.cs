using HelpMyStreetFE.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IVerificationService
    {
        public Task<ValidationResponse> ValidateUserAsync(ValidationRequest request, CancellationToken token = default);
    }
}
