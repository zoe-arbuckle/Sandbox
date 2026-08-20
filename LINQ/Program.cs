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

/*
 * =====================================
 * Distinct & Removing Duplicates
 * =====================================
 */
CustomConsoleLogs.Section("Distinct & Removing Duplicates");
List<string> emails = new List<string>
{
    "alice@example.com",
    "bob@example.com",
    "alice@example.com",
    "charlie@example.com",
    "bob@example.com",
    "diana@example.com",
    "alice@example.com"
};

// get all unique email addresses, sort them alphabetically, count the number of unique emails, and display each unique email
var uniqueEmails = emails.Distinct().OrderBy(e => e);
Console.WriteLine($"{uniqueEmails.Count()} unique emails");
foreach (var email in uniqueEmails)
{
    Console.WriteLine(email);
}

/*
 * =====================================
 * Join & Combining Collections
 * =====================================
 */
CustomConsoleLogs.Section("Join & Combining Collections");
var departments = new List<(int DeptId, string DeptName)>
{
    (1, "Engineering"),
    (2, "Sales"),
    (3, "HR")
};

var employeesWithDept = new List<(int EmpId, string Name, int DepartmentId)>
{
    (101, "Alice", 1),
    (102, "Bob", 2),
    (103, "Charlie", 1),
    (104, "Diana", 3),
    (105, "Eve", 1)
};

// match employees with departments
var joined = employeesWithDept.Join(departments,
    employee => employee.DepartmentId,
    department => department.DeptId,
    (e, d) => new { e.Name, d.DeptName }
    ).OrderBy(e => e.DeptName).ThenBy(e => e.Name);

foreach (var employee in joined)
{
    Console.WriteLine($"{employee.Name}: {employee.DeptName}");
}

/*
 * =====================================
 * Take, Skip & Pagination
 * =====================================
 */
CustomConsoleLogs.Section("Take, Skip & Pagination");
var productsToPaginate = new List<(string Name, decimal Price)>
{
    ("Laptop", 999m),
    ("Mouse", 25m),
    ("Keyboard", 75m),
    ("Monitor", 350m),
    ("Desk", 300m),
    ("Chair", 150m),
    ("Lamp", 45m),
    ("Headphones", 120m),
    ("Webcam", 80m),
    ("USB Cable", 10m)
};

var orderedProducts = productsToPaginate.OrderByDescending(p => p.Price);

IEnumerable<(string Name, decimal Price)> GetPage(int pageNumber, int pageSize) => orderedProducts.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
int TotalPages(int pageSize) => (orderedProducts.Count() + pageSize - 1) / pageSize;

const int PageSize = 3;

var page1 = GetPage(1, PageSize);
var page2 = GetPage(2, PageSize);

Console.WriteLine($"Page 1 of {TotalPages(PageSize)}:");
foreach (var item in page1)
    Console.WriteLine($"  {item.Name}: ${item.Price}");

Console.WriteLine($"\nPage 2 of {TotalPages(PageSize)}:");
foreach (var item in page2)
    Console.WriteLine($"  {item.Name}: ${item.Price}");

Console.WriteLine($"Total pages: {TotalPages(PageSize)}");


/*
 * =====================================
 * SelectMany & Flattening Nested Collections
 * =====================================
 */
CustomConsoleLogs.Section("SelectMany & Flattening Nested Collections");
var schools = new List<(string SchoolName, List<(string ClassName, List<string> Students)> Classes)>
{
    ("Lincoln High", new List<(string, List<string>)>
    {
        ("9A", new List<string> { "Alice", "Bob", "Charlie" }),
        ("9B", new List<string> { "Diana", "Eve" })
    }),
    ("Washington High", new List<(string, List<string>)>
    {
        ("10A", new List<string> { "Frank", "Grace", "Henry" }),
        ("10B", new List<string> { "Ivy", "Jack" })
    })
};

// flatten all students from all schools and classes to a single list
var allStudents = schools.SelectMany(
    school => school.Classes.SelectMany(
            c => c.Students,
            (Class, Student) => new { Class.ClassName, Student }
        ),
    (School, Class) => new { Class.Student, Class.ClassName, School.SchoolName }
    ).OrderBy(s => s.SchoolName).ThenBy(s => s.ClassName);

/* alternately, can use query syntax:
 * 
 * var allStudents = from school in schools
                  from @class in school.Classes
                  from student in @class.Students
                  orderby school.SchoolName, @class.ClassName
                  select new { Student, @class.ClassName, school.SchoolName };
 */

Console.WriteLine("Students with schools and classes");
foreach (var student in allStudents)
    Console.WriteLine($"{student.Student} is in class {student.ClassName} at school {student.SchoolName}");

/*
 * =====================================
 * Any, All & Validation
 * =====================================
 */
CustomConsoleLogs.Section("Any, All & Validation");
var users = new List<(string Name, int Age, bool IsActive, List<string> Roles)>
{
    ("Alice", 28, true, new List<string> { "Admin", "User" }),
    ("Bob", 35, false, new List<string> { "User" }),
    ("Charlie", 22, true, new List<string> { "Moderator", "User" }),
    ("Diana", 45, true, new List<string> { "Admin" }),
    ("Eve", 19, true, new List<string> { "User" })
};

Console.WriteLine($"Any user inactive: {users.Any(u => !u.IsActive)}");
Console.WriteLine($"All users 18+: {users.All(u => u.Age >= 18)}");
Console.WriteLine($"Any user has admin role: {users.Any(u => u.Roles.Any(u => u.Equals("Admin")))}");
Console.WriteLine($"All active users have at least one role: {users.All(u => u.Roles.Any())}");
Console.WriteLine($"Any user is under 21 and active: {users.Any(u => u.Age < 21 && u.IsActive)}");

/*
 * =====================================
 * Capstone
 * =====================================
 */
CustomConsoleLogs.Section("Capstone");
/*
 You're building a reporting system for an online store. You need to:

    Analyze customer orders
    Generate sales summaries by category
    Find top performing products
    Identify at-risk customers (haven't ordered recently)
    Create paginated reports
 */

var orders = new List<(int OrderId, string CustomerId, DateTime OrderDate, List<(string ProductName, string Category, int Quantity, decimal Price)> Items)>
{
    (1, "C001", new DateTime(2024, 1, 15), new List<(string, string, int, decimal)>
    {
        ("Laptop", "Electronics", 1, 999m),
        ("Mouse", "Electronics", 2, 25m)
    }),
    (2, "C002", new DateTime(2024, 2, 20), new List<(string, string, int, decimal)>
    {
        ("Desk", "Furniture", 1, 300m)
    }),
    (3, "C001", new DateTime(2024, 3, 10), new List<(string, string, int, decimal)>
    {
        ("Monitor", "Electronics", 1, 350m),
        ("Keyboard", "Electronics", 1, 75m)
    }),
    (4, "C003", new DateTime(2024, 1, 5), new List<(string, string, int, decimal)>
    {
        ("Chair", "Furniture", 3, 150m)
    }),
    (5, "C002", new DateTime(2024, 3, 25), new List<(string, string, int, decimal)>
    {
        ("Lamp", "Furniture", 2, 45m)
    }),
    (6, "C001", new DateTime(2024, 3, 28), new List<(string, string, int, decimal)>
    {
        ("Headphones", "Electronics", 1, 120m)
    })
};

// sales by category
var groupedByCategory = orders.SelectMany(o => o.Items).GroupBy(i => i.Category).OrderByDescending(c => c.Sum(c => c.Quantity * c.Price));
Console.WriteLine("Revenue per category");
foreach (var category in groupedByCategory)
    Console.WriteLine($"Category: {category.Key}, Products Sold: {category.Count()} Revenue: {category.Sum(c => c.Quantity * c.Price)}");

// top products
var groupedByProduct = orders.SelectMany(o => o.Items).GroupBy(i => i.ProductName).Where(i => i.Sum(p => p.Quantity) > 1).OrderByDescending(i => i.Sum(c => c.Quantity * c.Price));
Console.WriteLine("Top 3 products by revenue");
foreach (var product in groupedByProduct.Take(3))
    Console.WriteLine($"Category: {product.Key}, Revenue: {product.Sum(c => c.Quantity * c.Price)}");

// customer activity
var ordersPerCustomer = orders.GroupBy(c => c.CustomerId);
Console.WriteLine("Unique Customers");
foreach (var customer in ordersPerCustomer)
{
    Console.WriteLine($"{customer.Key}");
    Console.WriteLine($"Order Count: {customer.Count()}");
    Console.WriteLine($"Average order value: {Math.Round(customer.Average(c => c.Items.Sum(i => i.Quantity * i.Price)), 2, MidpointRounding.AwayFromZero)}");
    Console.WriteLine($"Orders in March 2024: {customer.Any(o => o.OrderDate.Month == 3 && o.OrderDate.Year == 2024)}");
}

// pagination
IEnumerable<(int OrderId, string CustomerId, DateTime OrderDate, List<(string ProductName, string Category, int Quantity, decimal Price)> Items)> GetOrdersPage(int pageNumber, int pageSize) => orders.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
int TotalOrdersPages(int pageSize) => (orders.Count() + pageSize - 1) / pageSize;

int size = 2;
var pageOne = GetOrdersPage(1, size);

Console.WriteLine($"Page 1 of {TotalOrdersPages(size)}:");
foreach (var order in pageOne) {
    Console.WriteLine($"Order: {order.OrderId}, Customer: {order.CustomerId}, OrderDate: {order.OrderDate}");
    foreach (var item in order.Items)
    {
        Console.WriteLine($"Item: {item.ProductName}, Category: {item.Category}, Quantity: {item.Quantity}, Price: {item.Price}");
    }
}