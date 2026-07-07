using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {
        private readonly CoffeeMachineRepository _repository;

        public CoffeeMachineController(CoffeeMachineRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_repository.GetCoffeeQuantities());
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            return Ok(_repository.GetCoffeePricesInCents());
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<int, int>> GetQuantity()
        {
            return Ok(_repository.GetCoinInventory());
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return BadRequest("Orden vacía.");

            if (request.Payment.TotalAmount <= 0)
                return BadRequest("Dinero insuficiente.");

            var (success, message) = _repository.BuyCoffee(request);

            if (!success)
                return BadRequest(message);

            return Ok(message);
        }
    }
}
