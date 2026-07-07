using ExamTwo.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class UnitTestingCoffeeMachine
{
    [Fact]
    public void BuyCoffee_EmptyOrder_ReturnsBadRequest()
    {
        // Arrange
        var db = new Database();
        var controller = new CoffeeMachineController(db);
        var request = new OrderRequest
        {
            Order = new Dictionary<string, int>(),
            Payment = new Payment { TotalAmount = 1000 }
        };

        // Act
        var result = controller.BuyCoffee(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Orden vacía.", badRequest.Value);
    }
}