﻿
# Command Buffer Helpers

[Repository On Github](https://github.com/Sibz/Com.Sibz.CommandBufferHelpers)

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
inputDeps = Entities.WithAll<MyComponent>().ForEach((Entity entity) => {
  concBuffer.RemoveComponent<MyComponent>(entity)
}).Schedule(inputDeps);

// Don't forget this
buffer.AddJobDependency(inputDeps);
```

To add another concurrent buffer to another job from, you need to force a new buffer
```c#
buffer.ForceNewBuffer()
```
This will make the next `.Concurrent (or .Buffer)` property access to generate a new buffer. 


### EntityCommandBuffer Extensions
#### CreateSingleton
These extensions apply to these command buffer classes, EntityCommandBuffers and their Concurrent version.
```c#
Entity entity = commandBuffer.CreateSingleton<MyComponent>();
// Or with data
Entity entity = commandBuffer.CreateSingleton(new MyComponent{Index = 1});
```
