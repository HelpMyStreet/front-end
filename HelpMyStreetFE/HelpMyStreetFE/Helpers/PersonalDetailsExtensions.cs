using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Helpers
{
    public static class PersonalDetailsExtensions
    {
        public static string FullName(this UserPersonalDetails userPersonalDetails)
        {
            return $"{userPersonalDetails.FirstName} {userPersonalDetails.LastName}";
        }

        public static IEnumerable<string> PhoneNumbers(this UserPersonalDetails userPersonalDetails)
        {
            return (new[] { userPersonalDetails.MobilePhone, userPersonalDetails.OtherPhone }).Where(a => !string.IsNullOrEmpty(a));
        }

        public static IEnumerable<string> PhoneNumbers(this RequestPersonalDetails requestPersonalDetails)
        {
            return (new[] { requestPersonalDetails.MobileNumber, requestPersonalDetails.OtherNumber }).Where(a => !string.IsNullOrEmpty(a));
        }

        public static string LocationSummary(this UserPersonalDetails userPersonalDetails)
        {
            return LocationSummary(userPersonalDetails?.Address?.Locality, userPersonalDetails?.Address?.Postcode);
        }

        public static string LocationSummary(this RequestPersonalDetails requestPersonalDetails)
        {
            return LocationSummary(requestPersonalDetails?.Address?.Locality, requestPersonalDetails?.Address?.Postcode);
        }

        public static string LocationSummary(string locality, string postcode)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(locality))
            {
                sb.Append(locality.ToTitleCase());
            }

            if (!string.IsNullOrEmpty(postcode))
            {
                sb.Append(" (");
                sb.Append(postcode.Split(' ').First().ToUpper());
                sb.Append(")");
            }

            return sb.ToString().Trim();
        }

        public static string CommaSeparatedAddress(this UserPersonalDetails userPersonalDetails)
        {
            string[] elements = new[]
            {
                userPersonalDetails.Address.AddressLine1,
                userPersonalDetails.Address.AddressLine2,
                userPersonalDetails.Address.AddressLine3,
                userPersonalDetails.Address.Locality.ToTitleCase(),
                userPersonalDetails.Address.Postcode
            };

            return string.Join(", ", elements.Where(e => !string.IsNullOrEmpty(e)));
        }

        public static string CommaSeparatedAddress(this RequestPersonalDetails requestPersonalDetails)
        {
            string[] elements = new[]
            {
                requestPersonalDetails.Address.AddressLine1,
                requestPersonalDetails.Address.AddressLine2,
                requestPersonalDetails.Address.AddressLine3,
                requestPersonalDetails.Address.Locality.ToTitleCase(),
                requestPersonalDetails.Address.Postcode
            };

            return string.Join(", ", elements.Where(e => !string.IsNullOrEmpty(e)));
        }
    }
}
