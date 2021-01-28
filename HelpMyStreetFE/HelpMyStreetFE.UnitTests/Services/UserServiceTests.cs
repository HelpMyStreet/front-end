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

namespace HelpMyStreetFE.UnitTests.Services
{
    class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<ILogger<HelpMyStreetFE.Services.Users.UserService>> _logger;
        private Mock<IMemDistCache<User>> _memDistCache;
        private Mock<IAddressService> _addressService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<HelpMyStreetFE.Services.Users.UserService>>();
            _memDistCache = new Mock<IMemDistCache<User>>();
            _addressService = new Mock<IAddressService>();
        }

        [Test]
        public void FormatName_EmptyString()
        {
            HelpMyStreetFE.Services.Users.UserService userService = new HelpMyStreetFE.Services.Users.UserService(_userRepository.Object, _logger.Object, _memDistCache.Object);

            Assert.AreEqual("", userService.FormatName(""));
        }

        [Test]
        public void FormatName_ShortName()
        {
            HelpMyStreetFE.Services.Users.UserService userService = new HelpMyStreetFE.Services.Users.UserService(_userRepository.Object, _logger.Object, _memDistCache.Object);

            Assert.AreEqual("A B C O'd", userService.FormatName(" a b C o'd"));
        }

        [Test]
        public void FormatName_MultiWordName()
        {
            HelpMyStreetFE.Services.Users.UserService userService = new HelpMyStreetFE.Services.Users.UserService(_userRepository.Object, _logger.Object, _memDistCache.Object);

            Assert.AreEqual("Martin Seamus MARTY McFly", userService.FormatName("    martin seamus   MARTY mcFly "));
        }
    }
}
