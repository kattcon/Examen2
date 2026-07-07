using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {

        private readonly Database _db;

        public CoffeeMachineController(Database db)
        {
            _db = db;
        }

        // esta
        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_db.keyValues);
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            return Ok(_db.keyValues2);
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<string, int>> GetQuantity()
        {
            return Ok(_db.keyValues3);
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return BadRequest("Orden vacía.");

            if (request.Payment.TotalAmount <= 0)
                return BadRequest("Dinero insuficiente.");

            try
            {
                foreach (var item in request.Order)
                {
                    if (!_db.keyValues2.ContainsKey(item.Key))
                        return BadRequest($"Café desconocido: {item.Key}.");
                }

                var totalCost = request.Order.Sum(o => _db.keyValues2[o.Key] * o.Value);

                if (request.Payment.TotalAmount < totalCost)
                    return BadRequest("Dinero insuficiente.");

                foreach (var coffee in request.Order)
                {
                    if (coffee.Value > _db.keyValues[coffee.Key])
                        return BadRequest($"No hay suficientes {coffee.Key} en la máquina.");
                }

                foreach (var coffee in request.Order)
                    _db.keyValues[coffee.Key] -= coffee.Value;

                var change = request.Payment.TotalAmount - totalCost;
                var result = $"Su vuelto es de: {change} colones. Desglose:";

                foreach (var coin in _db.keyValues3.Keys.OrderByDescending(c => c))
                {
                    var count = Math.Min(change / coin, _db.keyValues3[coin]);
                    if (count > 0)
                    {
                        result += $" {count} moneda de {coin},";
                        change -= coin * count;
                    }
                }

                if (change > 0)
                    return StatusCode(500, "No hay suficiente cambio en la máquina.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }

      
    

}
