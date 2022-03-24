using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using HelpMyStreet.Cache;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Services;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.UnitTests.Helpers
{
    class DueDateTypeExtensionsTests
    {
        [Test]
        public void HasDate_AllValuesCovered()
        {
            foreach (DueDateType val in Enum.GetValues(typeof(DueDateType)))
            {
                _ = val.HasDate();
            }
        }

        [Test]
        public void HasStartTime_AllValuesCovered()
        {
            foreach (DueDateType val in Enum.GetValues(typeof(DueDateType)))
            {
                _ = val.HasStartTime();
            }
        }

        [Test]
        public void HasEndTime_AllValuesCovered()
        {
            foreach (DueDateType val in Enum.GetValues(typeof(DueDateType)))
            {
                _ = val.HasEndTime();
            }
        }
    }
}
