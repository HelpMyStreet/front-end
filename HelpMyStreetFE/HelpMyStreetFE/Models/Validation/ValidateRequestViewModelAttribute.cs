using HelpMyStreetFE.Models.RequestHelp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateRequestViewModelAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            List<string> erroredFields = new List<string>();
            RequestHelpViewModel requestHelp = value as RequestHelpViewModel;

            if (requestHelp.HelpRequest == null) erroredFields.Add($"{nameof(requestHelp.HelpRequest)} cannot be null or empty");
            if (requestHelp.JobRequest == null) erroredFields.Add($"{nameof(requestHelp.JobRequest)} cannot be null or empty");

            ValidateRecipient(requestHelp.HelpRequest.Recipient, erroredFields, false) ;
            ValidateRecipient(requestHelp.HelpRequest.Requestor, erroredFields, true);

            if (string.IsNullOrEmpty(requestHelp.JobRequest.SupportActivity))
                erroredFields.Add($"{nameof(requestHelp.JobRequest.SupportActivity)} cannot be null or empty");

            if(requestHelp.HelpRequest.ForRequestor == false)
            {
                ValidateAddress(requestHelp.HelpRequest.Requestor.Address, erroredFields, true);            
            }
            else
            {
                ValidateAddress(requestHelp.HelpRequest.Requestor.Address, erroredFields);
            }

            ValidateAddress(requestHelp.HelpRequest.Recipient.Address, erroredFields);

            if (erroredFields.Count > 0)
            {
                ErrorMessage = "Validation Failed for the following reasons: " + erroredFields.Select(x => x + ",");
                return false;
            }
            return true;
        }

        private void ValidateRecipient(HelpRequestPersonDetails person, List<string> erroredFields, bool validateEmail)
        {
            if (string.IsNullOrEmpty(person.Firstname)) erroredFields.Add($"{nameof(person.Firstname)} cannot be null or empty");
            if (string.IsNullOrEmpty(person.Lastname)) erroredFields.Add($"{nameof(person.Lastname)} cannot be null or empty");
            if (string.IsNullOrEmpty(person.Mobile) && string.IsNullOrEmpty(person.AltNumber)) erroredFields.Add("Either a mobile or alternative number must be provided");
            if (validateEmail) {
              if (string.IsNullOrEmpty(person.Email)) erroredFields.Add($"{nameof(person.Email)} cannot be null or empty");
            }                                      
            
        }
        private void ValidateAddress(HelpRequestAddress address, List<string> erroredFields, bool onlyValidatePostcode = false)
        {
            if (!onlyValidatePostcode)
            {
                if (string.IsNullOrEmpty(address.Addressline1)) erroredFields.Add($"{nameof(address.Addressline1)} cannot be null or empty");
                if (string.IsNullOrEmpty(address.Locality)) erroredFields.Add($"{nameof(address.Locality)} cannot be null or empty");
            }
            if (string.IsNullOrEmpty(address.Postcode) && string.IsNullOrEmpty(address.Postcode)) erroredFields.Add("Either a mobile or alternative number must be provided");         
        }

    }
}
