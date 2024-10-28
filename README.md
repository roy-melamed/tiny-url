# tiny-url
TinyUrl implemented in ASP.NET with MongoDB and LRUCache

Why I Chose This Approach:
I decided to go with an LRU (Least Recently Used) cache because it's simple and effective for managing the cache size. 
With this approach, I can ensure that the cache doesn't grow too large and cause memory overflow issues.

Description:
I implemented an LRU cache to control the size of our cache. 
This cache stores a fixed number of recently accessed URLs and removes the least recently used URLs when the cache is full.

Reasoning:

Concurrency and Multi-threading: The LRU cache handles concurrency well, ensuring that my code is consistent and safe even in multi-threaded environments.
Request Collapsing: By storing frequently accessed URLs in memory, the cache minimizes database hits, 
making my system faster by serving URLs directly from memory when they're requested multiple times.
Advantages:

Thread-Safety: The LRU cache implementation ensures that my code is thread-safe, which is crucial for handling multiple requests simultaneously.
Reduced Database Access: With the cache in place, I can minimize database hits, leading to faster response times for users accessing the same URLs repeatedly.
Efficient Memory Usage: By limiting the cache size, I can prevent memory overflow issues and make sure that my system uses memory efficiently.
Disadvantages:

Cache Eviction Policy: Sometimes, the LRU cache may remove items that are still frequently accessed, which could affect the cache's efficiency.
Fixed Size Limitation: Since the cache has a fixed size, less frequently accessed items may get evicted, impacting performance if the cache size isn't optimal.
