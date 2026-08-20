# 4-Week C# Backend Engineering Study Plan

## Overview
This plan targets your critical knowledge gaps identified through interview questions. Focus: **Production-grade system design, observability, resilience, and distributed systems.**

**Time Commitment:** 5-7 hours/week (focus over breadth)  
**Format:** Theory → Practice → Applied Project

---

## Week 1: Resilience Patterns & Distributed Systems Fundamentals

### Goals
- Master retry strategies, circuit breakers, bulkheads
- Understand transient vs permanent failures
- Learn Polly library (industry standard)

### Daily Breakdown

**Day 1-2: Retry & Backoff Strategies**
- Read: [Polly Docs - Retry Policy](https://github.com/App-vNext/Polly/wiki/Retry)
- Concept: Exponential backoff, jitter, max retries
- Why it matters: Your IoT system calling external APIs
- Practice: Write a method that retries with exponential backoff (no library first)

```csharp
// PRACTICE: Implement this without Polly
public async Task<T> RetryWithBackoffAsync<T>(
    Func<Task<T>> operation, 
    int maxRetries = 3)
{
    // Hint: Use Math.Pow(2, attempt) for exponential backoff
    // Add jitter: new Random().Next(0, (int)TimeSpan.FromSeconds(Math.Pow(2, attempt)).TotalMilliseconds)
}
```

**Day 3: Circuit Breaker Pattern**
- Read: [Polly Docs - Circuit Breaker](https://github.com/App-vNext/Polly/wiki/Circuit-Breaker)
- Concept: Open → Half-Open → Closed states
- Why it matters: Prevent cascading failures in microservices
- Scenario: Your Device API is down. Without circuit breaker, you retry 10,000x/sec. With circuit breaker, you fail fast after 5 failures.

```csharp
// PRACTICE: Understand this flow
var policy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
    .CircuitBreakerAsync<HttpResponseMessage>(
        handledEventsAllowedBeforeBreaking: 5,  // Break after 5 failures
        durationOfBreak: TimeSpan.FromSeconds(30)  // Try again after 30s
    );
```

**Day 4-5: Bulkhead Pattern & Practical Implementation**
- Read: [Polly Docs - Bulkhead](https://github.com/App-vNext/Polly/wiki/Bulkhead)
- Concept: Isolate resources to prevent one failure from crashing everything
- Why it matters: 10,000 devices calling your API shouldn't exhaust connection pool
- Practice: Build a device telemetry fetcher with bulkhead (max 100 concurrent)

```csharp
// PRACTICE: Implement this
public class DeviceTelemetryFetcher
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;
    
    public DeviceTelemetryFetcher()
    {
        _policy = Policy
            .BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 100,  // Max 100 concurrent
                maxQueuingActions: 1000)  // Queue up to 1000 waiting requests
            .WrapAsync(/* retry policy */);
    }
    
    public async Task FetchAllDevicesAsync(List<int> deviceIds)
    {
        // Use _policy to execute FetchDeviceAsync for each device
    }
}
```

**Day 6: Polly Wrap & Combine Policies**
- Practice: Combine retry + circuit breaker + bulkhead
- This is what production code looks like

```csharp
// PRACTICE: Combine all three
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .Or<TimeoutException>()
    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
    .CircuitBreakerAsync<HttpResponseMessage>(5, TimeSpan.FromSeconds(30));

var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(100, 1000);

var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, bulkheadPolicy);

// Now use combinedPolicy.ExecuteAsync(() => FetchDeviceAsync(id));
```

**Day 7: Review & Applied Exercise**
- Scenario: You have 10,000 IoT devices. Each needs telemetry from 3 external APIs. Design the resilience strategy.
- Write pseudocode/code showing:
  - Which failures are retryable
  - Circuit breaker thresholds
  - Bulkhead limits
  - Fallback behavior (what if all APIs fail?)

### Resources
- Polly GitHub: https://github.com/App-vNext/Polly
- Microsoft Docs: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-resilient-application
- Video: "Polly for Resilient Applications" (search YouTube)

### Validation
By end of Week 1, you should be able to:
- ✅ Explain exponential backoff vs linear retry
- ✅ Describe when circuit breaker opens/closes
- ✅ Design a resilient API call with Polly
- ✅ Identify bottlenecks in concurrent systems

---

## Week 2: Database Performance & Query Optimization

### Goals
- Master indexing strategies
- Optimize queries with Entity Framework Core
- Understand connection pooling & query execution plans
- Identify N+1 problems

### Daily Breakdown

**Day 1-2: Indexing & Query Plans**
- Read: [SQL Server Execution Plans](https://docs.microsoft.com/en-us/sql/relational-databases/performance/execution-plans)
- Concept: Clustered vs nonclustered indexes, composite indexes
- Why it matters: Query going from 100ms to 10ms is the difference between scale and failure

```sql
-- PRACTICE: Create indexes for your IoT telemetry table
CREATE TABLE DeviceTelemetry (
    TelemetryId INT PRIMARY KEY,
    DeviceId INT,
    Timestamp DATETIME,
    Temperature DECIMAL,
    Quality DECIMAL
)

-- What index would help this query?
SELECT * FROM DeviceTelemetry 
WHERE DeviceId = 123 
  AND Timestamp BETWEEN '2026-01-01' AND '2026-08-20'
ORDER BY Timestamp DESC

-- Answer: Composite index on (DeviceId, Timestamp)
```

**Day 3: Entity Framework Core Query Patterns**
- Read: [EF Core Query Performance](https://docs.microsoft.com/en-us/ef/core/performance/query-performance)
- Problem: N+1 queries
  - Bad: Loop through 100 devices, query DB for each → 101 queries
  - Good: Load all device data in 1 query with .Include()

```csharp
// PRACTICE: Spot the N+1 problem
var devices = await _dbContext.Devices.ToListAsync();
foreach (var device in devices)
{
    var telemetry = await _dbContext.DeviceTelemetry
        .Where(t => t.DeviceId == device.DeviceId)
        .ToListAsync();
    // ❌ This queries DB 101 times (1 for devices + 100 for telemetry)
}

// SOLUTION: Use .Include()
var devices = await _dbContext.Devices
    .Include(d => d.Telemetry)  // Load telemetry in same query
    .ToListAsync();
foreach (var device in devices)
{
    var telemetry = device.Telemetry;  // Already loaded
    // ✅ 1 query total
}
```

**Day 4: Projections & Avoiding Large Data Transfers**
- Concept: Select only columns you need, not entire rows
- Why it matters: Fetching 100 rows × 50 columns vs 100 rows × 5 columns = 10x bandwidth savings

```csharp
// PRACTICE: Rewrite this query efficiently
var devices = await _dbContext.Devices
    .Include(d => d.Telemetry)
    .Include(d => d.MaintenanceHistory)
    .Include(d => d.Users)
    .ToListAsync();

// BETTER: Select only needed columns
var deviceSummary = await _dbContext.Devices
    .Select(d => new 
    {
        d.DeviceId,
        d.Name,
        LatestTemperature = d.Telemetry.OrderByDescending(t => t.Timestamp).First().Temperature,
        MaintenanceDue = d.MaintenanceHistory.Any(m => m.DueDate < DateTime.Now)
        // Ignore: Users, old telemetry, etc.
    })
    .ToListAsync();
```

**Day 5: Connection Pooling & Connection Strings**
- Concept: Connection pooling prevents opening/closing 1000s of DB connections
- Why it matters: Opening connection = 50-100ms overhead

```csharp
// PRACTICE: Understand this connection string
"Server=myserver;Database=mydb;User Id=sa;Password=pwd;Min Pool Size=5;Max Pool Size=100;"
// Min Pool Size: Keep 5 connections always ready
// Max Pool Size: Never open more than 100 connections
// If all 100 are in use, requests wait (queue) or fail

// ANTI-PATTERN:
using (var connection = new SqlConnection(connString))
{
    // Opens connection (slow)
    await connection.OpenAsync();
    // Do work
} // Closes connection (but connection pool may reuse it)

// PATTERN (EF Core handles this for you):
await _dbContext.Devices.ToListAsync();  // Reuses connection from pool
```

**Day 6-7: Applied Exercise**
- Scenario: Telemetry query is taking 2 seconds for 10,000 devices
- Debug checklist:
  1. Is there an N+1 query? (use SQL Profiler to check)
  2. Are indexes missing? (check execution plan)
  3. Is query selecting too many columns? (profile bandwidth)
  4. Is connection pool exhausted? (check pool metrics)
- Write a query optimization proposal

### Resources
- [SQL Server Query Tuning](https://www.sqlshack.com/en/sql-server-query-hints/)
- [EF Core Performance](https://docs.microsoft.com/en-us/ef/core/performance/query-performance)
- Tool: SQL Server Management Studio → "Display Estimated Execution Plan"
- Tool: EF Core Profiler or MiniProfiler to see generated SQL

### Validation
By end of Week 2, you should be able to:
- ✅ Create composite indexes for common query patterns
- ✅ Spot N+1 problems in code
- ✅ Use `.Include()` and `.Select()` effectively
- ✅ Read an execution plan and identify bottlenecks
- ✅ Explain why connection pooling matters

---

## Week 3: Observability & Distributed Tracing

### Goals
- Master Application Insights (Azure's observability tool)
- Implement distributed tracing with correlation IDs
- Set up meaningful metrics and alerts
- Diagnose production issues using traces

### Daily Breakdown

**Day 1-2: Metrics, Logs, Traces (The Three Pillars)**
- Read: [Observability in Microservices](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- Concept: The 3 pillars of observability
  - **Metrics:** Aggregated numbers (req/sec, avg latency, error rate)
  - **Logs:** Detailed event records ("User 123 called API at 10:05:32.123")
  - **Traces:** Request flow across services ("Request A → Service B → DB query → Cache miss")

```csharp
// PRACTICE: Instrument a method with all three
public class DeviceTelemetryService
{
    private readonly ILogger<DeviceTelemetryService> _logger;
    private readonly TelemetryClient _telemetryClient;
    
    public async Task<DeviceTelemetry> GetTelemetryAsync(int deviceId)
    {
        var startTime = DateTime.UtcNow;
        
        // METRIC: Track how often this is called
        _telemetryClient.TrackEvent("GetTelemetry_Called", 
            new Dictionary<string, string> { { "deviceId", deviceId.ToString() } });
        
        try
        {
            // LOG: Detailed event
            _logger.LogInformation("Fetching telemetry for device {DeviceId}", deviceId);
            
            var telemetry = await _dbContext.DeviceTelemetry
                .Where(t => t.DeviceId == deviceId)
                .FirstOrDefaultAsync();
            
            // METRIC: Track success and duration
            var duration = DateTime.UtcNow - startTime;
            _telemetryClient.TrackEvent("GetTelemetry_Success", null, 
                new Dictionary<string, double> { { "DurationMs", duration.TotalMilliseconds } });
            
            return telemetry;
        }
        catch (Exception ex)
        {
            // METRIC & LOG: Track errors
            _logger.LogError(ex, "Failed to fetch telemetry for device {DeviceId}", deviceId);
            _telemetryClient.TrackException(ex, 
                new Dictionary<string, string> { { "deviceId", deviceId.ToString() } });
            throw;
        }
    }
}
```

**Day 3: Correlation IDs & Request Tracking**
- Concept: Assign unique ID to each request, pass it through all services
- Why it matters: When user reports bug, you can grep logs for correlation ID and see the entire flow

```csharp
// PRACTICE: Implement correlation ID middleware
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";
    
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers
            .FirstOrDefault(h => h.Key == CorrelationIdHeader).Value
            .FirstOrDefault() ?? Guid.NewGuid().ToString();
        
        context.Items[CorrelationIdHeader] = correlationId;
        context.Response.Headers.Add(CorrelationIdHeader, correlationId);
        
        // Pass to all downstream services
        await _next(context);
    }
}

// Usage in Startup:
public void Configure(IApplicationBuilder app)
{
    app.UseMiddleware<CorrelationIdMiddleware>();
    // ...
}

// In your service, inject IHttpContextAccessor to read correlation ID
public class DeviceTelemetryService
{
    public async Task<DeviceTelemetry> GetTelemetryAsync(int deviceId)
    {
        var correlationId = _httpContextAccessor.HttpContext.Items["X-Correlation-ID"];
        _logger.LogInformation("Fetching telemetry. CorrelationId: {CorrelationId}", correlationId);
        // Every log now includes correlation ID
    }
}
```

**Day 4-5: Application Insights Setup & Queries**
- Read: [Application Insights for ASP.NET Core](https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core)
- Setup: Add NuGet package, configure in Startup.cs
- Practice: Write queries to answer real questions

```csharp
// Setup in Program.cs (ASP.NET Core 6+)
builder.Services.AddApplicationInsightsTelemetry();

// Add custom telemetry
services.AddSingleton<TelemetryClient>();
```

```kusto
// PRACTICE: Kusto Query Language (used in Application Insights)
// Question: What's the error rate for device telemetry API?

requests
| where name == "GET /devices/telemetry"
| summarize 
    TotalRequests = count(),
    FailedRequests = countif(success == false),
    ErrorRate = (countif(success == false) * 100.0 / count())
| project ErrorRate

// Question: What's the p95 latency?
requests
| where name == "GET /devices/telemetry"
| summarize p95_latency = percentile(duration, 95)

// Question: Show all requests for device 123 in the last hour
traces
| where customDimensions.deviceId == "123"
| where timestamp > ago(1h)
| project timestamp, message, customDimensions
```

**Day 6: Setting Up Meaningful Alerts**
- Practice: Create alerts for your IoT system

```csharp
// Alert if error rate > 5%
// Alert if p95 latency > 2000ms
// Alert if 10,000 devices → queue depth > 50,000
// These prevent you from discovering problems via customer complaints
```

**Day 7: Simulate & Debug Production Issue**
- Scenario: Device telemetry requests taking 5 seconds (should be 100ms)
- Using correlation IDs + traces, walk through the investigation
- Write a diagnostic query that identifies the bottleneck

### Resources
- [Application Insights Overview](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Kusto Query Language](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/)
- [Distributed Tracing with Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/distributed-tracing)

### Validation
By end of Week 3, you should be able to:
- ✅ Instrument code with metrics, logs, traces
- ✅ Implement and use correlation IDs
- ✅ Write Kusto queries to diagnose issues
- ✅ Identify bottlenecks using Application Insights
- ✅ Design alert thresholds for a system

---

## Week 4: Distributed Systems Patterns & Integration

### Goals
- Master async messaging patterns (queues, events)
- Handle idempotency & eventual consistency
- Design for failure in distributed systems
- Apply all previous learnings to capstone project

### Daily Breakdown

**Day 1-2: Message Queues & Pub-Sub Patterns**
- Read: [Azure Service Bus Patterns](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)
- Concept: At-least-once delivery, message acknowledgment, dead-letter queues

```csharp
// PRACTICE: Implement publisher
public class DeviceAlertPublisher
{
    private readonly IAsyncClient<IMessageSession> _serviceBusClient;
    
    public async Task PublishAlertAsync(int deviceId, string alertType)
    {
        var message = new ServiceBusMessage
        {
            Body = new BinaryData(JsonSerializer.Serialize(new
            {
                AlertId = Guid.NewGuid(),  // Unique ID for idempotency
                DeviceId = deviceId,
                AlertType = alertType,
                Timestamp = DateTime.UtcNow
            }))
        };
        
        await _sender.SendMessageAsync(message);
    }
}

// PRACTICE: Implement subscriber
public class DeviceAlertSubscriber : IMessageHandler
{
    private readonly IAlertService _alertService;
    
    public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var alert = JsonSerializer.Deserialize<Alert>(args.Message.Body.ToString());
            
            // IDEMPOTENCY: Check if we've already processed this alert
            if (await _alertService.HasBeenProcessedAsync(alert.AlertId))
            {
                _logger.LogInformation("Alert already processed, skipping");
                await args.CompleteMessageAsync();  // Acknowledge
                return;
            }
            
            // Process the alert
            await _alertService.SendToUserAsync(alert);
            
            // ACKNOWLEDGE: Tell queue we're done
            await args.CompleteMessageAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process alert");
            // Message will be retried automatically
            // After max retries, goes to Dead Letter Queue
        }
    }
}
```

**Day 3: Idempotency & Eventual Consistency**
- Concept: Messages might be processed multiple times → design for it
- Pattern: Unique message ID, store processed IDs, check before processing

```csharp
// PRACTICE: Implement idempotent operation
public class AlertService
{
    public async Task SendToUserAsync(Alert alert)
    {
        // Check if we've already sent this alert
        var previousResult = await _dbContext.ProcessedAlerts
            .FirstOrDefaultAsync(p => p.AlertId == alert.AlertId);
        
        if (previousResult != null)
        {
            _logger.LogInformation("Alert {AlertId} already processed", alert.AlertId);
            return;  // Exit early
        }
        
        // Send the alert
        await _notificationService.SendAsync(alert.UserId, alert.Message);
        
        // Record that we've processed it
        await _dbContext.ProcessedAlerts.AddAsync(new ProcessedAlert
        {
            AlertId = alert.AlertId,
            ProcessedAt = DateTime.UtcNow,
            Status = "Sent"
        });
        await _dbContext.SaveChangesAsync();
    }
}
```

**Day 4: Saga Pattern for Distributed Transactions**
- Concept: Coordinate multi-step operations across services
- Why it matters: If Service A succeeds but Service B fails, you need to rollback

```
SCENARIO: Device firmware update
1. Service A: Download firmware
2. Service B: Validate firmware
3. Service C: Apply to device
4. Service D: Log update in audit trail

If step 3 fails, rollback steps 1-2
```

```csharp
// PRACTICE: Implement choreography-based saga
public class FirmwareUpdateSaga
{
    // Step 1: Download firmware
    public async Task StartFirmwareUpdateAsync(int deviceId, string firmwareVersion)
    {
        var download = await _downloadService.DownloadAsync(firmwareVersion);
        
        // Publish event for next step
        await _eventBus.PublishAsync(new FirmwareDownloadedEvent 
        { 
            DeviceId = deviceId, 
            FirmwareId = download.Id 
        });
    }
    
    // Step 2: Validate (subscribed to FirmwareDownloadedEvent)
    public async Task ValidateFirmwareAsync(FirmwareDownloadedEvent evt)
    {
        var isValid = await _validationService.ValidateAsync(evt.FirmwareId);
        
        if (isValid)
        {
            await _eventBus.PublishAsync(new FirmwareValidatedEvent { ... });
        }
        else
        {
            // Rollback: publish compensation event
            await _eventBus.PublishAsync(new FirmwareUpdateFailedEvent { ... });
        }
    }
    
    // Handle failure: cleanup and notify
    public async Task HandleFirmwareUpdateFailedAsync(FirmwareUpdateFailedEvent evt)
    {
        await _downloadService.DeleteAsync(evt.FirmwareId);
        await _notificationService.NotifyUserAsync(evt.DeviceId, "Update failed");
    }
}
```

**Day 5-6: Capstone Project - Design Complete System**
- **Scenario:** Design a scalable, reliable device monitoring system that:
  - Ingests telemetry from 100,000 IoT devices
  - Sends alerts in real-time for anomalies (< 5 second SLA)
  - Stores historical data for 1 year (analytics)
  - Handles infrastructure failures gracefully
  - Allows updates with zero downtime

- **Requirements:**
  1. **Resilience:** What happens if alert service is down?
  2. **Observability:** How do you diagnose latency issues?
  3. **Performance:** How do you handle 100K devices × 10 requests/sec = 1M req/sec?
  4. **Consistency:** What if device sends duplicate telemetry?

- **Deliverable:** Write a detailed design document including:
  - Architecture diagram (services, databases, queues, caches)
  - Data flow (with resilience annotations)
  - Monitoring strategy (metrics, alerts, traces)
  - Disaster scenarios (what if database goes down? How do you recover?)

**Day 7: Review & Interview Prep**
- Go through your capstone design
- Answer: "Walk me through how your system handles a database outage"
- Answer: "How do you deploy a new version without downtime?"
- Answer: "What's your observability strategy?"

### Resources
- [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/)
- [Saga Pattern](https://microservices.io/patterns/data/saga.html)
- [Eventual Consistency](https://www.allthingsdistributed.com/2008/12/eventually_consistent.html)

### Validation
By end of Week 4, you should be able to:
- ✅ Design pub-sub systems with at-least-once delivery
- ✅ Implement idempotent operations
- ✅ Explain the Saga pattern and when to use it
- ✅ Design a complete distributed system
- ✅ Answer production-grade interview questions

---

## Cross-Week Resources & Tools

### Essential Tools
1. **SQL Server Management Studio** — query profiling, execution plans
2. **Azure Data Studio** — query editor for Azure
3. **Application Insights** — observability (built into Azure)
4. **Postman** — API testing and load testing
5. **Fiddler/Telerik** — HTTP traffic inspection
6. **dotTrace** — .NET profiling (see where time is spent)

### NuGet Packages to Explore
```xml
<!-- Resilience -->
<PackageReference Include="Polly" Version="8.0.0" />

<!-- Observability -->
<PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.20.0" />
<PackageReference Include="Serilog.Extensions.Logging.File" Version="2.0.0" />

<!-- Testing -->
<PackageReference Include="Moq" Version="4.17.0" />
<PackageReference Include="xunit" Version="2.4.1" />

<!-- Message Queue -->
<PackageReference Include="Azure.Messaging.ServiceBus" Version="7.10.0" />
```

### Daily Study Routine
1. **Morning (30 min):** Read theory/documentation
2. **Midday (60-90 min):** Write practice code
3. **Evening (30 min):** Review and write notes
4. **Weekend:** Apply learnings to personal project or capstone

---

## Validation Checkpoints

### After Week 1: Resilience Patterns
```
Can you explain why this code is wrong?
var tasks = deviceIds.Select(id => FetchAsync(id)).ToList();
await Task.WhenAll(tasks);  // No retry, no circuit breaker, no backpressure
```

### After Week 2: Database Performance
```
This query takes 5 seconds for 1,000 devices. Optimize it:
var devices = await _db.Devices.ToListAsync();
foreach (var device in devices)
{
    var telemetry = await _db.Telemetry
        .Where(t => t.DeviceId == device.Id)
        .ToListAsync();
}
```

### After Week 3: Observability
```
User reports: "My device alerts are delayed."
Walk through your diagnostic steps using Application Insights.
```

### After Week 4: Distributed Systems
```
Design: 100K devices, each sends 1 alert/minute.
System must guarantee no alert is dropped or duplicated.
How do you architect this?
```

---

## Success Criteria

**By the end of 4 weeks, you should:**
- ✅ Confidently discuss resilience patterns in interviews
- ✅ Optimize database queries and identify bottlenecks
- ✅ Design observability strategies for production systems
- ✅ Understand distributed systems trade-offs
- ✅ Walk through real-world scenarios (scaling, failures, deployments)
- ✅ Connect all concepts to your IoT/monitoring background

---

## Next Steps (Post-Study Plan)

### Month 2-3: Deepen Knowledge
- Implement capstone project as real code in your repo
- Create practice interview scenarios
- Study specific technologies (Azure, Kubernetes, etc.)

### Ongoing: Stay Current
- Follow releases: .NET 8/9, Entity Framework updates
- Read articles: Azure Architecture, System Design
- Practice: LeetCode system design questions

---

## Quick Reference: Key Equations

**Exponential Backoff:**
```
delay = baseDelay * Math.Pow(2, attemptNumber) + jitter
Example: 1s, 2s, 4s, 8s, 16s
```

**Database Indexing:**
```
Index benefit = Full table scan time / Indexed query time
Good index: 100-1000x faster
```

**Throughput Calculation:**
```
devices * requests_per_device_per_sec * size_per_request = bandwidth
100K * 10 * 1KB = 1GB/sec → Need caching/aggregation
```

**Availability Calculation:**
```
5 nines = 99.999% = 26 seconds/year downtime
4 nines = 99.99% = 52 minutes/year downtime
Each additional nine = 10x harder to achieve
```

---

Good luck! Track your progress, and feel free to revisit weeks if concepts aren't clicking. The goal is mastery, not speed.
