namespace ExamTwo.Controllers
{
    public class CoffeeMachineRepository
    {
        private readonly Database _db;

        public CoffeeMachineRepository(Database db)
        {
            _db = db;
        }

        public Dictionary<string, int> GetCoffeeQuantities()
        {
            return _db.CoffeeQuantities;
        }

        public Dictionary<string, int> GetCoffeePricesInCents()
        {
            return _db.CoffeePricesInCents;
        }

        public Dictionary<int, int> GetCoinInventory()
        {
            return _db.CoinInventory;
        }

        public (bool success, string message) BuyCoffee(OrderRequest request)
        {
            foreach (var item in request.Order)
            {
                if (!_db.CoffeePricesInCents.ContainsKey(item.Key))
                    return (false, $"Café desconocido: {item.Key}.");
            }

            var totalCost = request.Order.Sum(o => _db.CoffeePricesInCents[o.Key] * o.Value);

            if (request.Payment.TotalAmount < totalCost)
                return (false, "Dinero insuficiente.");

            foreach (var coffee in request.Order)
            {
                if (coffee.Value > _db.CoffeeQuantities[coffee.Key])
                    return (false, $"No hay suficientes {coffee.Key} en la máquina.");
            }

            foreach (var coffee in request.Order)
                _db.CoffeeQuantities[coffee.Key] -= coffee.Value;

            var change = request.Payment.TotalAmount - totalCost;
            var result = $"Su vuelto es de: {change} colones. Desglose:";

            foreach (var coin in _db.CoinInventory.Keys.OrderByDescending(c => c))
            {
                var count = Math.Min(change / coin, _db.CoinInventory[coin]);
                if (count > 0)
                {
                    result += $" {count} moneda de {coin},";
                    change -= coin * count;
                }
            }

            if (change > 0)
                return (false, "No hay suficiente cambio en la máquina.");

            return (true, result);
        }
    }
}