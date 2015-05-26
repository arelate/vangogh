using Microsoft.VisualStudio.TestTools.UnitTesting;

using GOG.Interfaces;
using GOG.Controllers;
using GOG.SharedControllers;
using GOG.SharedModels;

namespace GoodOfflineGames.Tests
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        private IUriController uriController;
        private IStringNetworkController stringNetworkController;
        private IConsoleController consoleController;
        private AuthenticationController authenticationController;
        private ICredentials correctCredentials;
        private ICredentials wrongCredentials;

        public AuthenticationControllerTests()
        {
            uriController = new UriController(); // no need to mock UriController
            stringNetworkController = new MockNetworkController(uriController);
            consoleController = new MockConsoleController();
            correctCredentials = new Settings() { Username = "username", Password = "password" };
            wrongCredentials = new Settings() { Username = "wrongusername", Password = "wrongpassword" };

            authenticationController = new AuthenticationController(uriController, stringNetworkController, consoleController);
        }

        [TestMethod]
        public void AuthenticationControllerAuthorizesWithCorrectCredentials()
        {
            Assert.IsTrue(authenticationController.AuthorizeOnSite(correctCredentials).Result);
        }

        [TestMethod]
        public void AuthenticationControllerFailsAuthorizationWithWrongCredentials()
        {
            Assert.IsFalse(authenticationController.AuthorizeOnSite(wrongCredentials).Result);
        }
    }
}
