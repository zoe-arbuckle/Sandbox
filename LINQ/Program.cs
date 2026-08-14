using Common;

/*
 * =====================================
 * LINQ Basics & Query Syntax
 * =====================================
 */
CustomConsoleLogs.Section("LINQ Basics & Query Syntax");
List<int> integers = [10, 25, 8, 42, 15, 33, 7];
// Define the query expression
IEnumerable<int> numbersGreaterThan15 =
    from integer in integers
    where integer > 15
    select integer;

// Execute the query
foreach (var i in numbersGreaterThan15)
{
    Console.WriteLine(i);
}

/*
 * =====================================
 * Select & Transformation
 * =====================================
 */
CustomConsoleLogs.Section("Select & Transformation");

List<string> names = new List<string> { "Alice", "Bob", "Charlie", "Diana" };

// create a new collection where each name is transformed to uppercase
IEnumerable<string> asUppercase = from name in names
                                  select name.ToUpper();
// create a new collection with anonymous objects containing original name and string length
var transformed = from name in names
                  select new { Original = name, Length = name.Length };

// print results
Console.WriteLine("Uppercase:");
foreach (var name in asUppercase)
    Console.WriteLine(name);

Console.WriteLine("Transformed:");
foreach (var obj in transformed)
    Console.WriteLine($"{obj.Original} length is {obj.Length}");

/*
 * =====================================
 * Combining Where & Select (Chaining)
 * =====================================
 */
CustomConsoleLogs.Section("Combining Where & Select (Chaining)");

List<int> prices = new List<int> { 5, 15, 8, 25, 12, 30, 10 };

var discountedPrices = from price in prices
                       where price >= 10 // filter for prices greater than or equal to 10
                       select new { OriginalPrice = price, DiscountedPrice = price * 0.8 }; // apply a 20% discount to each price & create objects with OriginalPrice and DiscountedPrice

// print results
Console.WriteLine("Discounted Prices:");
foreach (var price in discountedPrices)
    Console.WriteLine($"{price.OriginalPrice} discounted to {price.DiscountedPrice}");

/*
 * =====================================
 * FirstOrDefault, SingleOrDefault & Aggregation
 * =====================================
 */
CustomConsoleLogs.Section("FirstOrDefault, SingleOrDefault & Aggregation");
