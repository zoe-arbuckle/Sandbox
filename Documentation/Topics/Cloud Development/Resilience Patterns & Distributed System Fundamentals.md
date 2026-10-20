# Resilience Patterns & Distributed Systems Fundamentals
## References
- [Polly Docs](https://www.pollydocs.org/strategies)

## Notes
### Resilience Strategies
Resilience strategies are designed to execute user-defined callbcks while adding an extra layer of resilience. Polly provides an API to construct the resilience
pipelines that enable resilience strategies.
There are two main groups of resilience strategies: reactive and proactive.

**Reactive** strategies handle specific exceptions that are thrown or results returned by the callbacks

**Proactive** strategies can cancel or reject the execution of callbacks 

Failures are also categorized into two categories: transient and permanent.

**Transient** failures may resolve if you wait and retry - eg. network failures.

**Permanent** failures will not resolve with repeated attempts - eg. authentication failures

#### Reactive Strategies
##### Retry
Many faults are transient - if a short delay will fix the issue, retrying the same request after that delay will fix the problem.
Between retry attempts, the strategy waits a specified amount of time, until the maximum number of retries is reached. Often it is recommended to use an
exponential backoff, increasing the wait each time (2 ^ attempt seconds) to avoid repeatedly making requests to a struggling service. Randomizing the 
wait slightly, known as jitter, is used to prevent high numbers of clients from retrying at exactly the same time.

Retry should not be used for periodic execution, it is meant for brief transient failures nd not to run indefinitely at a specified interval.

##### Circuit Breaker
Similar to its electrical safety namesake, the circuit breaker strategy is designed to stop execution if the underlying service is unhealthy. After N consecutive failures
or if the sampled failure-success ratio exceeds a defined threshold then the circuit breaker prevents new executions with a `BrokenCircuitException`. After a preset duration
the circuit breaker will allow another attempt to check if the service self-healed. If it fails, new requests will continue to be blocked but if it succeeds then new 
requests will be allowed. This prevents cascading failure if the API is down.

If minimum throughput is not reached during the sampling duration, then the failure ratio will be ignored and the circuit will not brek even if all the executions fail.

Circuit breaker rethrows all exceptions and does not manage retries, but can be combined with a retry strategy.

##### Fallback
Fallback strategy provides a substitute if the execution of the callback fails. This is usually a last resort if all other strategies fail. For example, a fallback can 
provide a sensible default value if multiple retries fail. However, fallback should not be used to replace the thrown exception to avoid silent or misleading failures.

##### Hedging
Hedging strategy enables re-execution of the callback if the previous execution is taking too long. This does cause increased resource utilization, but can boost the 
responsiveness of the system. You can either run the original callback again or specify a new callback for hedged attempts. You should not start background work when
exeucting actions using hedging, because multiple parallel tasks might be spawned.

The hedging strategy supports multiple concurrency modes. Latency mode triggers additional executions when the initial one takes too long to complete. Fallback mode
only allows a single executino to proceed at a given time. Parallel mode starts all executions simultaneously and waits for the fastest completion. Dynamic mode gives the 
flexibility to control how the hedging strategy behaves for each execution.

#### Proactive Strategies
##### Timeout
Timeout cancels the execution if it does not complete within the specified timeout period. Timeout must respect the incoming cancellation token, and not throw the `TimeoutRejectedException`
when the original token is cancelled.

##### Rate Limiter
Rate limiter controls the number of operations that can pass through it. This can be used to control inbound load rate (rate limiter) or outbound load (concurrency limiter).
Rate limiters can also be partitioned by a key, to further apply the rate limits to each user for example.