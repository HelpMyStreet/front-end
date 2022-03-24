using HelpMyStreet.Utils.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.UnitTests.Helpers
{
    public class SupportActivityHelpersTests
    {
        [Test]
        public void IncludeInDefaultSortAndFilterSet_AllValuesCovered()
        {
            foreach (SupportActivities val in Enum.GetValues(typeof(SupportActivities)))
            {
                _ = val.IncludeInDefaultSortAndFilterSet();
            }
        }

    }
}
