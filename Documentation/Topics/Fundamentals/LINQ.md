# LINQ
## References
- Github Copilot
- [Overview of LINQ - Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)

## Notes
### LINQ Basics & Query Syntax
LINQ, or *Language-Integrated Query*, is a set of technologies integrating SQL-like query capabilities directly into C#.

Queries are written in a declarative way using either *query syntax* or *method syntax*.

**Query Syntax Example**
```csharp
// Create a data source
int[] ages = [7, 49, 10, 28, 93, 24];

// Define the query expression
IEnumerable<int> ageQuery = 
	from age in ages
	where age > 30
	select age;

// Execute the query
foreach (var i in ageQuery)
{
	Console.Write(i + " ");
}

// Output: 49 93
```

**Method Syntax Example**
```csharp
int[] ages = [7, 49, 10, 28, 93, 24];

// Define the query expression
IEnumerable<int> ageQuery = ages.Where(num => num > 30);

// Execute the query
foreach (var i in ageQuery)
{
    Console.Write(i + " ");
}

// Output: 49 93
```

#### Common Mistakes
- Using `foreach` to manually filter or transform when LINQ already does this
- Not understanding that LINQ queries are often lazy (execute when results are enumerated, not when declared)
- Mixing query and method syntax inconsistently

### Select & Transformation
`Select` transforms each element in a collection to a new form - creating a new type, a computed value, or a modified object instead of just filtering.

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };

// Example 1: Double each number
var doubled = from num in numbers
              select num * 2;

Console.WriteLine("Doubled:");
foreach (var num in doubled)
    Console.WriteLine(num);  // Output: 2, 4, 6, 8, 10

// Example 2: Convert to strings with custom format
var asStrings = from num in numbers
                select $"Number: {num}";

Console.WriteLine("\nAs strings:");
foreach (var str in asStrings)
    Console.WriteLine(str);  // Output: Number: 1, Number: 2, etc.

// Example 3: Create anonymous objects
var transformed = from num in numbers
                  select new { Original = num, Squared = num * num };

Console.WriteLine("\nSquared:");
foreach (var obj in transformed)
    Console.WriteLine($"{obj.Original} squared = {obj.Squared}");
```

#### Common Mistakes
- Confusing `Select` (transformation) with `Where` (filtering)
- Forgetting that `Select` can change the data type of collection items
- Not realizing you can nest complex logic inside `Select`
- Using `Select` when you only need to access properties (ok in some scenarios)

### Combining Where & Select (Chaining)
`Where` and `Select` can be combined to filter data and then transform only the data that needs to be transformed.

```csharp
List<int> numbers = new List<int> { 5, 12, 3, 18, 7, 25, 9 };

// Example 1: Filter then transform (query syntax)
var result1 = from num in numbers
              where num > 10
              select num * 2;

Console.WriteLine("Numbers > 10, doubled:");
foreach (var num in result1)
    Console.WriteLine(num);  // Output: 24, 36, 50

// Example 2: Same thing in method syntax (more common for chaining)
var result2 = numbers.Where(n => n > 10).Select(n => n * 2);

Console.WriteLine("\nSame result, method syntax:");
foreach (var num in result2)
    Console.WriteLine(num);

// Example 3: Complex chaining with strings
List<string> words = new List<string> { "apple", "banana", "cat", "dog", "elephant" };

var longWordsUppercase = from word in words
                         where word.Length > 3
                         select word.ToUpper();

Console.WriteLine("\nWords with length > 3, uppercase:");
foreach (var word in longWordsUppercase)
    Console.WriteLine(word);  // Output: APPLE, BANANA, ELEPHANT
```

#### Common Mistakes
- Wrong order - it is more efficient to filter first, then transform
- Forgetting LINQ operations can be chained together
- Using nested loops instead of method chaining
- Forgetting each operation returns a new `IEnumerable`

### FirstOrDefault, SingleOrDefault, & Aggregation
These can be used when you need a **single value** or **summary** of your data, and are terminal operations eg. query is executed immediately.

- `FirstOrDefault()` - Gets the first element, or a default value if none exist
- `SingleOrDefault()` - Gets the single element, or default if none/multiple exist (throws if multiple)
- `Count()` - Counts elements (optionally with a condition)
- `Sum()`, `Average()`, `Max()`, `Min()` - Aggregate numeric values
- `Any()` - Checks if any element matches a condition
- `All()` - Checks if all elements match a condition

```csharp
List<int> numbers = new List<int> { 5, 10, 15, 20, 25 };

// FirstOrDefault
int first = numbers.FirstOrDefault();
Console.WriteLine($"First: {first}");  // Output: 5

int firstOver12 = numbers.FirstOrDefault(n => n > 12);
Console.WriteLine($"First over 12: {firstOver12}");  // Output: 15

// Count with condition
int countOver15 = numbers.Count(n => n > 15);
Console.WriteLine($"Count over 15: {countOver15}");  // Output: 2

// Sum, Average, Max, Min
int sum = numbers.Sum();
double average = numbers.Average();
int max = numbers.Max();
int min = numbers.Min();

Console.WriteLine($"Sum: {sum}, Avg: {average}, Max: {max}, Min: {min}");
// Output: Sum: 75, Avg: 15, Max: 25, Min: 5

// Any and All
bool hasOver20 = numbers.Any(n => n > 20);
bool allOver5 = numbers.All(n => n > 5);

Console.WriteLine($"Has over 20: {hasOver20}");  // Output: True
Console.WriteLine($"All over 5: {allOver5}");     // Output: True

// Empty collection example
List<int> empty = new List<int>();
int emptyFirst = empty.FirstOrDefault();  // Returns 0 (default for int)
Console.WriteLine($"Empty list first: {emptyFirst}");  // Output: 0
```

#### Common Mistakes
- Using `First()` instead of `FirstOrDefault()` when the sequence might be empty
- Confusing `FirstOrDefault()` with `SingleOrDefault()`: first gets the first, single expects exactly one
- Forgetting these operations **execute immediately**
- Using `Count()` on large collections when `Any()` would be more efficient
- Not handling the potential `null` return value from `FirstOrDefault()`

### OrderBy, ThenBy & Sorting
LINQ provides the following methods to order collections:
- `OrderBy()` - sort in ascending order
- `OrderByDescending()` - sort in descending order
- `ThenBy()`/ `ThenByDescending()` - add secondary sort criteria (must be after `OrderBy`)

This allows you to sort by any property or computed value, chain multiple sort criteria, and makes syntax clean and readable.

```csharp
List<int> numbers = new List<int> { 5, 2, 8, 1, 9, 3 };

// Example 1: Simple ascending sort
var ascending = numbers.OrderBy(n => n);
Console.WriteLine("Ascending:");
foreach (var num in ascending)
    Console.WriteLine(num);  // Output: 1, 2, 3, 5, 8, 9

// Example 2: Descending sort
var descending = numbers.OrderByDescending(n => n);
Console.WriteLine("\nDescending:");
foreach (var num in descending)
    Console.WriteLine(num);  // Output: 9, 8, 5, 3, 2, 1

// Example 3: Sort strings by length, then alphabetically
List<string> words = new List<string> { "apple", "cat", "banana", "dog", "elephant" };

var sorted = words.OrderBy(w => w.Length)      // First by length (ascending)
                  .ThenBy(w => w);              // Then alphabetically

Console.WriteLine("\nBy length, then alphabetically:");
foreach (var word in sorted)
    Console.WriteLine(word);  // Output: cat, dog, apple, banana, elephant

// Example 4: Sort complex data (students by grade, then by name)
var students = new List<(string Name, int Grade)>
        {
            ("Alice", 85),
            ("Bob", 92),
            ("Charlie", 85),
            ("Diana", 78)
        };

var sortedStudents = students.OrderByDescending(s => s.Grade)  // Highest grade first
                             .ThenBy(s => s.Name);               // Then alphabetically

Console.WriteLine("\nStudents by grade (desc), then name (asc):");
foreach (var student in sortedStudents)
    Console.WriteLine($"{student.Name}: {student.Grade}");
// Output: Bob: 92, Alice: 85, Charlie: 85, Diana: 78
```

#### Common Mistakes
- Using `OrderBy()` without `ThenBy()` when you need multiple sort levels
- Forgetting that `ThenBy()` must follow `OrderBy()`
- Trying to sort with `Where` or `Select` before `OrderBy()` - inefficient, sort should come last
- Not realizing `OrderBy()` returns `IOrderedEnumerable` 
- Sorting by string without considering case sensitivity

### GroupBy & Grouping Data
`GroupBy()` organizes data into groups based on a key. This results in a collection of collections where items with the same key are grouped together.
This is useful for aggregating data by category, generating summaries, and analyzing patterns in data.

```csharp
List<int> numbers = new List<int> { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };

// Example 1: Group by number value
var grouped = numbers.GroupBy(n => n);

Console.WriteLine("Numbers grouped by value:");
foreach (var group in grouped)
{
    Console.WriteLine($"Number {group.Key}: appears {group.Count()} times");
}
// Output:
// Number 1: appears 1 times
// Number 2: appears 2 times
// Number 3: appears 3 times
// Number 4: appears 4 times

// Example 2: Group words by length
List<string> words = new List<string> { "cat", "dog", "elephant", "ant", "bird", "tiger" };

var groupedByLength = words.GroupBy(w => w.Length);

Console.WriteLine("\nWords grouped by length:");
foreach (var group in groupedByLength)
{
    Console.WriteLine($"Length {group.Key}: {string.Join(", ", group)}");
}
// Output:
// Length 3: cat, dog, ant
// Length 4: bird
// Length 5: tiger
// Length 8: elephant

// Example 3: Group with aggregation
List<int> scores = new List<int> { 85, 92, 78, 85, 88, 78, 92, 95 };

var scoreGroups = scores.GroupBy(s => s)
                        .OrderByDescending(g => g.Key);

Console.WriteLine("\nScores with frequency:");
foreach (var group in scoreGroups)
{
    Console.WriteLine($"Score {group.Key}: occurs {group.Count()} times");
}
```

#### Common Mistakes
- `GroupBy()` returns `IGrouping<TKey, TElement>`, not a list
- not iterating through groups correctly - each group needs its own `foreach`
- confusing `GroupBy()` with sorting
- trying to use `FirstOrDefault()` or `Count()` on a group without iterating properly
- LINQ methods can be applied to each group independently

### Distinct & Removing Duplicates
`Distinct()` removes duplicate elements from a collection. This can be useful if you want unique values from a dataset,
need to eliminate redundancy, or are combining multiple data sources with potential overlap.
This works for simple types (int, string) out of the box, but for custom objects you need to implement `IEquatable<T>` or provide
a custom comparer.

```csharp
// Example 1: Distinct on simple types
List<int> numbers = new List<int> { 1, 2, 2, 3, 3, 3, 4, 1, 5 };

var uniqueNumbers = numbers.Distinct();
Console.WriteLine("Unique numbers:");
foreach (var num in uniqueNumbers)
    Console.WriteLine(num);  // Output: 1, 2, 3, 4, 5

// Example 2: Distinct on strings
List<string> names = new List<string> { "Alice", "Bob", "Alice", "Charlie", "Bob" };

var uniqueNames = names.Distinct();
Console.WriteLine("\nUnique names:");
foreach (var name in uniqueNames)
    Console.WriteLine(name);  // Output: Alice, Bob, Charlie

// Example 3: Distinct after projection
List<string> words = new List<string> { "apple", "apricot", "banana", "avocado" };

var firstLetters = words.Select(w => w[0]).Distinct();
Console.WriteLine("\nUnique first letters:");
foreach (var letter in firstLetters)
    Console.WriteLine(letter);  // Output: a, b

// Example 4: Distinct with OrderBy for sorted unique values
List<int> unsorted = new List<int> { 5, 2, 5, 1, 2, 3 };

var sortedUnique = unsorted.Distinct().OrderBy(n => n);
Console.WriteLine("\nSorted unique numbers:");
foreach (var num in sortedUnique)
    Console.WriteLine(num);  // Output: 1, 2, 3, 5
```

#### Common Mistakes
- Calling `Distinct()` on complex objects and expecting it to work without custom comparison logic
- forgetting `Distinct()` uses the default equality comparer
- not realizing `Distinct()` doesn't sort results
- using `Distinct()` multiple times when it should only be used once
- confusing `Distinct()` with `GroupBy()`

### Join & Combining Collections
`Join()` combines data from two collections based on a matching key. This is a more efficient way to correlate related data versus using a nested loop.
`Join()` can be useful for matching customers with their orders, finding which students are in which classes, or correlating product IDs with product names.

```csharp
// Example 1: Simple join
List<int> ids = new List<int> { 1, 2, 3 };
List<(int Id, string Name)> names = new List<(int, string)>
        {
            (1, "Alice"),
            (2, "Bob"),
            (3, "Charlie")
        };

var joined = ids.Join(
    names,
    id => id,                           // outer key
    name => name.Id,                    // inner key
    (id, name) => new { id, name.Name } // result
);

Console.WriteLine("Joined data:");
foreach (var item in joined)
    Console.WriteLine($"ID: {item.id}, Name: {item.Name}");
// Output: ID: 1, Name: Alice | ID: 2, Name: Bob | ID: 3, Name: Charlie

// Example 2: Join with more complex scenario
var students = new List<(int Id, string Name)>
        {
            (1, "Alice"),
            (2, "Bob"),
            (3, "Charlie")
        };

var enrollments = new List<(int StudentId, string Course)>
        {
            (1, "Math"),
            (1, "English"),
            (2, "Science"),
            (3, "Math")
        };

var studentCourses = students.Join(
    enrollments,
    s => s.Id,
    e => e.StudentId,
    (s, e) => new { s.Name, e.Course }
);

Console.WriteLine("\nStudent Courses:");
foreach (var item in studentCourses)
    Console.WriteLine($"{item.Name}: {item.Course}");
// Output: Alice: Math, Alice: English, Bob: Science, Charlie: Math

// Example 3: Join with OrderBy for organized output
var orderedCourses = students.Join(
    enrollments,
    s => s.Id,
    e => e.StudentId,
    (s, e) => new { s.Name, e.Course }
).OrderBy(x => x.Name).ThenBy(x => x.Course);

Console.WriteLine("\nStudent Courses (sorted):");
foreach (var item in orderedCourses)
    Console.WriteLine($"{item.Name}: {item.Course}");
```

#### Common Mistakes
- Forgetting that `Join()` performs an inner join (only matching items)
- Not understanding the parameter order: `Join(inner, outerKeySelector, innerKeySelector, resultSelector)`
- Using nested `foreach` loops instead of `Join()` (inefficient)
- Confusing `Join()` with `SelectMany()` (different purposes)
- Not realizing the join key must match on both sides

### Take, Skip & Pagination
`Take()` and `Skip()` are used for pagination and limiting results.
`Skip(n)` skips the first `n` elements, and `Take(n)` takes only the first `n` elements. Together they can be used for pagination,
eg. `Skip(pageSize * (pageNumber -  1)).Take(pageSize)`. This is good for displaying results in batches, implementing "Load More" functionality,
and limiting API responses.

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Example 1: Take first 3
var firstThree = numbers.Take(3);
Console.WriteLine("First 3:");
foreach (var num in firstThree)
    Console.WriteLine(num);  // Output: 1, 2, 3

// Example 2: Skip first 3, then take 3
var middleThree = numbers.Skip(3).Take(3);
Console.WriteLine("\nSkip 3, take 3:");
foreach (var num in middleThree)
    Console.WriteLine(num);  // Output: 4, 5, 6

// Example 3: Pagination simulation
List<string> items = Enumerable.Range(1, 25)
    .Select(i => $"Item {i}")
    .ToList();

int pageSize = 5;
int pageNumber = 2;  // Get page 2

var page = items.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

Console.WriteLine($"\nPage {pageNumber} (5 items per page):");
foreach (var item in page)
    Console.WriteLine(item);
// Output: Item 6, Item 7, Item 8, Item 9, Item 10

// Example 4: Skip at end (last 3 items)
var exceptLast = numbers.SkipLast(3);
Console.WriteLine("\nAll except last 3:");
foreach (var num in exceptLast)
    Console.WriteLine(num);  // Output: 1, 2, 3, 4, 5, 6, 7

// Example 5: Take with OrderBy (proper pagination)
var students = new List<(string Name, int Grade)>
        {
            ("Alice", 85),
            ("Bob", 92),
            ("Charlie", 78),
            ("Diana", 88),
            ("Eve", 95)
        };

var topThree = students.OrderByDescending(s => s.Grade)
                       .Take(3);

Console.WriteLine("\nTop 3 students by grade:");
foreach (var student in topThree)
    Console.WriteLine($"{student.Name}: {student.Grade}");
```

#### Common Mistakes
- Calling `Take()` before `Skip()`
- Using `Take()` without `Skip()` when you need pagination
- Forgetting `Skip()` doesn't throw an error if `n` exceeds collection size
- not sorting before pagination, which can cause inconsistent results
- using `First()` or `Last()` when `Take()` would be more appropriate

### SelectMany & Flattening Nested Collections
`SelectMany()` flattens nested collections into a single sequence. This can be used to combine lists from multiple sources, 
flatten parent-child relationshiops, or project one-to-many relationships.

```csharp
// Example 1: Simple flattening
List<List<int>> nestedNumbers = new List<List<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5 },
            new List<int> { 6, 7, 8, 9 }
        };

var flattened = nestedNumbers.SelectMany(list => list);
Console.WriteLine("Flattened numbers:");
foreach (var num in flattened)
    Console.WriteLine(num);  // Output: 1, 2, 3, 4, 5, 6, 7, 8, 9

// Example 2: SelectMany with transformation
List<string> sentences = new List<string>
        {
            "Hello World",
            "LINQ is great",
            "C Sharp rocks"
        };

var allWords = sentences.SelectMany(s => s.Split(' '));
Console.WriteLine("\nAll words:");
foreach (var word in allWords)
    Console.WriteLine(word);
// Output: Hello, World, LINQ, is, great, C, Sharp, rocks

// Example 3: Flattening parent-child relationships
var departments = new List<(string Name, List<string> Employees)>
        {
            ("Engineering", new List<string> { "Alice", "Bob", "Charlie" }),
            ("Sales", new List<string> { "Diana", "Eve" }),
            ("HR", new List<string> { "Frank" })
        };

var allEmployees = departments.SelectMany(d => d.Employees);
Console.WriteLine("\nAll employees:");
foreach (var emp in allEmployees)
    Console.WriteLine(emp);
// Output: Alice, Bob, Charlie, Diana, Eve, Frank

// Example 4: SelectMany with index and result projection
var result = departments.SelectMany(
    d => d.Employees,
    (dept, emp) => new { Department = dept.Name, Employee = emp }
);

Console.WriteLine("\nDepartments with employees:");
foreach (var item in result)
    Console.WriteLine($"{item.Employee} works in {item.Department}");
```

#### Common Mistakes
- Confusing `SelectMany()` with `Select()` - `Select()` keeps nesting
- Not understanding that `SelectMany()` produces a cross-join when used with multiple sequences
- Forgetting to handle the selector parameter correctly
- using nested loops instead of `SelectMany()`
- Not realizing `SelectMany()` can change the result type

# [Example Project](https://github.com/zoe-arbuckle/Sandbox/tree/master/LINQ)