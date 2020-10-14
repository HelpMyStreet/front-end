using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreetFE.Helpers;
using NUnit.Framework;

namespace HelpMyStreetFE.UnitTests.Helpers
{
    class StringHelperTests
    {
        [Test]
        public void ToTitleCase_EmptyString()
        {
            Assert.AreEqual("", "".ToTitleCase());
        }

        [Test]
        public void ToTitleCase_Sentence()
        {
            Assert.AreEqual("The Quick Brown Fox Jumped Over The Lazy Dog", "the QUICK bROWN fOx jumped over THE LAZY Dog".ToTitleCase());
        }
    }
}
