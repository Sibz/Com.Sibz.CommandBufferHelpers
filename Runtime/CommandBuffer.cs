using System;
using Unity.Entities;
using Unity.Jobs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Sibz.CommandBufferHelpers
{
    public class CommandBuffer<T>
    where T: EntityCommandBufferSystem
    {
        private readonly T bufferSystem;
        private EntityCommandBuffer commandBuffer;

        public EntityCommandBuffer Buffer =>
            !commandBuffer.IsCreated ? (commandBuffer = bufferSystem.CreateCommandBuffer()) : commandBuffer;

        public EntityCommandBuffer.Concurrent Concurrent =>
            Buffer.ToConcurrent();

        public CommandBuffer(World world)
        {
            bufferSystem = world.GetExistingSystem<T>();
            if (bufferSystem is null)
            {
                throw new NullReferenceException($"{this.GetType().Name}: Can not be created in world ({world.Name}) as buffer system of type {typeof(T).Name} does not exist.");
            }
        }

        public void AddJobDependency(JobHandle jobHandle) => bufferSystem.AddJobHandleForProducer(jobHandle);
    }

    public class BeginInitCommandBuffer : CommandBuffer<BeginInitializationEntityCommandBufferSystem>
    {
        public BeginInitCommandBuffer(World world) : base(world)
        {
        }
    }

    public class EndInitCommandBuffer : CommandBuffer<EndInitializationEntityCommandBufferSystem>
    {
        public EndInitCommandBuffer(World world) : base(world)
        {
        }
    }

    public class BeingSimCommandBuffer : CommandBuffer<BeginSimulationEntityCommandBufferSystem>
    {
        public BeingSimCommandBuffer(World world) : base(world)
        {
        }
    }

    public class EndSimCommandBuffer : CommandBuffer<EndSimulationEntityCommandBufferSystem>
    {
        public EndSimCommandBuffer(World world) : base(world)
        {
        }
    }

    public class BeginPresCommandBuffer : CommandBuffer<BeginPresentationEntityCommandBufferSystem>
    {
        public BeginPresCommandBuffer(World world) : base(world)
        {
        }
    }
}