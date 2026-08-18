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
List<int> scores = new List<int> { 45, 78, 92, 65, 88, 55, 91, 72 };

// highest score
int max = scores.Max();
Console.WriteLine($"Highest score: {max}");
// lowest score
int min = scores.Min();
Console.WriteLine($"Lowest score: {min}");
// average score (2 decimal places)
double average = Math.Round(scores.Average(), 2, MidpointRounding.AwayFromZero);
Console.WriteLine($"Average score: {average}");
// count of scores 80 or above
int scoresGreaterOrEqual80 = scores.Count(s => s >= 80);
Console.WriteLine($"Count of scores 80 or above: {scoresGreaterOrEqual80}");
// Whether any score is below 50
bool anyBelow50 = scores.Any(s => s < 50);
Console.WriteLine($"Any score below 50: {anyBelow50}");
// Whether all scores are 40 or above
bool allAbove40 = scores.All(s => s >= 40);
Console.WriteLine($"All scores above 40: {allAbove40}");
// The first score that's 85 or above
int firstAbove85 = scores.FirstOrDefault(s => s >= 85);
Console.WriteLine($"First score 85 or above: {firstAbove85}");

/*
 * =====================================
 * OrderBy, ThenBy & Sorting
 * =====================================
 */
CustomConsoleLogs.Section("OrderBy, ThenBy & Sorting");
var employees = new List<(string Name, string Department, int Salary)>
{
    ("Alice", "Engineering", 85000),
    ("Bob", "Sales", 65000),
    ("Charlie", "Engineering", 90000),
    ("Diana", "Sales", 75000),
    ("Eve", "Engineering", 80000)
};

// primary sort - by department, alphabetically ascending
// secondary sort - by salary, descending

var orderedEmployees = employees.OrderBy(s => s.Department).ThenByDescending(s => s.Salary);
Console.WriteLine("Employees by department (alphabetically ascending), then salary (descending)");
foreach (var employee in orderedEmployees)
{
    Console.WriteLine($"Name: {employee.Name}, Department: {employee.Department}, Salary: {employee.Salary}");
}

/*
 * =====================================
 * GroupBy & Grouping Data
 * =====================================
 */
CustomConsoleLogs.Section("GroupBy & Grouping Data");
var products = new List<(string Name, string Category, decimal Price)>
{
    ("Laptop", "Electronics", 999m),
    ("Mouse", "Electronics", 25m),
    ("Desk", "Furniture", 300m),
    ("Chair", "Furniture", 150m),
    ("Monitor", "Electronics", 350m),
    ("Lamp", "Furniture", 45m)
};

// group products by category & sort alphabetically
var groupedProducts = products.GroupBy(s => s.Category).OrderBy(s => s.Key);
// for each category, display
//      category name
//      products in the category
//      count of products
//      average price, rounded to 2 decimals
foreach (var category in groupedProducts)
{
    Console.WriteLine($"Category: {category.Key}");
    Console.WriteLine($"Products: {string.Join(", ", category.Select(s => s.Name))}");
    Console.WriteLine($"Count: {category.Count()}");
    Console.WriteLine($"Average Price: {Math.Round(category.Average(p => p.Price), 2, MidpointRounding.AwayFromZero)}");
}