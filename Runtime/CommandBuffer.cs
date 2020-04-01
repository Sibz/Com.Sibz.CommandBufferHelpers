using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Sibz.CommandBufferHelpers
{
    public interface ICommandBuffer
    {
        EntityCommandBuffer.Concurrent Concurrent { get; }
        EntityCommandBuffer Buffer { get; }
        void AddJobDependency(JobHandle jobHandle);
    }
    public class CommandBuffer<T> : ICommandBuffer
        where T : EntityCommandBufferSystem
    {
        private readonly T bufferSystem;
        private EntityCommandBuffer commandBuffer;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private List<EntityCommandBuffer> pendingBuffersList;
#else
        private NativeList<EntityCommandBuffer> pendingBuffersList;
#endif

        public EntityCommandBuffer Buffer =>
            // ReSharper disable once PossibleNullReferenceException
            commandBuffer.IsCreated && !DidPlayback
                ? commandBuffer
                : commandBuffer = bufferSystem.CreateCommandBuffer();

        private bool DidPlayback => !pendingBuffersList.Contains(commandBuffer);

        public EntityCommandBuffer.Concurrent Concurrent =>
            Buffer.ToConcurrent();

        public CommandBuffer(World world)
        {
            bufferSystem = world.GetExistingSystem<T>();
            if (bufferSystem is null)
            {
                throw new NullReferenceException(
                    $"{GetType().Name}: Can not be created in world ({world.Name}) as buffer system of type {typeof(T).Name} does not exist.");
            }

            PropertyInfo propInfo;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((propInfo = typeof(EntityCommandBufferSystem)
                    .GetProperty("PendingBuffers", BindingFlags.NonPublic | BindingFlags.Instance)) is null ||
                ((pendingBuffersList = propInfo.GetValue(bufferSystem) as List<EntityCommandBuffer>) is null)
            )
                throw new InvalidOperationException(
                    $"{GetType().Name}: Unable to ascertain if buffer has been played back");
#else
            if ((propInfo = typeof(T)
                    .GetProperty("PendingBuffers", BindingFlags.NonPublic | BindingFlags.Instance)) is null ||
                !(pendingBuffersList = (NativeList<EntityCommandBuffer>)propInfo.GetValue(bufferSystem)).IsCreated
            )
                throw new InvalidOperationException($"{GetType().Name}: Unable to ascertain if buffer has been played back");
#endif
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