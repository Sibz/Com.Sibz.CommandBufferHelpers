# Command Buffer Helpers

### CommandBuffer classes
These create a command buffer and cache it, if the command buffer is processed, it creates a new one. 
Thus you can use the same property and it will be a fresh command buffer each frame.  
Named after the making Entities buffer systems, but with shorter names.  
`BeginInitCommandBuffer`  
`EndInitCommandBuffer`  
`BeingSimCommandBuffer`  
`EndSimCommandBuffer`  
`BeginPresCommandBuffer`

**Usage**  
```c#
// Declare in class
BeginPresCommandBuffer buffer;

// Define in constructor (or OnCreate for component systems)
buffer = new BeginPresCommandBuffer(World);
```
```c#
// Use directly
Entity entity = buffer.Buffer.CreateEntity();
```
```c#
// Or use conncurent to pass into job
var job = new job { buffer = Buffer.Concurrent }

// Job System Foreach variation
var concBuffer = buffer.Concurrent;
Entities.WithAll<MyComponent>().ForEach((Entity entity) => {
  concBuffer.RemoveComponent<MyComponent>(entity)
});
```

### EntityCommandBuffer Extensions
#### CreateSingleton
```c#
Entity entity = commandBuffer.CreateSingleton<MyComponent>();
// Or with data
Entity entity = commandBuffer.CreateSingleton(new MyComponent{Index = 1});
```
