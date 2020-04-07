using HelpMyStreetFE.Enums.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Validation
{
    public class ValidationResponse
    {
        public ValidationStatus Status { get; set; }
        public string Message { get; set; }
        public static ValidationResponse Error(string message = null) => new ValidationResponse { Status = ValidationStatus.UnexepectedError, Message = message };
        public static ValidationResponse Unauthorized(string message = null) => new ValidationResponse { Status = ValidationStatus.Unauthorized, Message = message };
        public static ValidationResponse ValidationFailed(string message = null) => new ValidationResponse { Status = ValidationStatus.ValidationFailed, Message = message };
        public static ValidationResponse UnexepectedError(string message = null) => new ValidationResponse { Status = ValidationStatus.UnexepectedError, Message = message };
        public static ValidationResponse Success() => new ValidationResponse { Status = ValidationStatus.Success };
    }
}
