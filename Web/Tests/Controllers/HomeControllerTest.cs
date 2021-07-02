using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Controllers;

namespace IQI.Intuition.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var environmentMock = new Mock<IActionContext>();
            environmentMock.SetupGet(mock => mock.CurrentAccount)
                .Returns(new Account("TEST"));
            environmentMock.SetupGet(mock => mock.CurrentUser)
                .Returns(new AccountUser(environmentMock.Object.CurrentAccount, "TEST"));
            environmentMock.SetupGet(mock => mock.CurrentFacility)
                .Returns(new Facility(environmentMock.Object.CurrentAccount, "TEST"));

            var controller = new HomeController(
                environmentMock.Object, 
                new Mock<IModelMapper>().Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}