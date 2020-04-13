using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

#if !ENABLE_UNITY_COLLECTIONS_CHECKS
using Unity.Collections;
#endif

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
        Action NewBuffer { get; set; }
        World World { set; }
        void AddJobDependency(JobHandle jobHandle);
        void ForceNewBuffer();
    }

    public class CommandBuffer<T> : ICommandBuffer
        where T : EntityCommandBufferSystem
    {
        private T bufferSystem;
        private EntityCommandBuffer commandBuffer;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private List<EntityCommandBuffer> pendingBuffersList;
#else
        private NativeList<EntityCommandBuffer> pendingBuffersList;
#endif

        private bool forceNewBuffer;
        private bool haveBeenInitialised;

        public World World
        {
            set
            {
                if (haveBeenInitialised)
                {
                    throw new InvalidOperationException("Can only set World once.");
                }

                haveBeenInitialised = true;
                Init(value);
            }
        }

        public EntityCommandBuffer Buffer
        {
            get
            {
                // ReSharper disable once PossibleNullReferenceException
                bool shouldNotGetNew = commandBuffer.IsCreated && !DidPlayback && !forceNewBuffer;
                if (shouldNotGetNew)
                {
                    return commandBuffer;
                }

                forceNewBuffer = false;
                commandBuffer = bufferSystem.CreateCommandBuffer();
                NewBuffer?.Invoke();
                return commandBuffer;
            }
        }

        private bool DidPlayback
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                for (int i = 0; i < pendingBuffersList.Count; i++)
#else
                for (int i = 0; i < pendingBuffersList.Length; i++)
#endif
                {
                    if (pendingBuffersList[i].Equals(commandBuffer))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public EntityCommandBuffer.Concurrent Concurrent =>
            Buffer.ToConcurrent();

        public void ForceNewBuffer()
        {
            forceNewBuffer = true;
        }

        public CommandBuffer()
        {
        }

        public CommandBuffer(World world)
        {
            World = world;
        }

        private void Init(World world)
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
                (pendingBuffersList = propInfo.GetValue(bufferSystem) as List<EntityCommandBuffer>) is null
            )
            {
                throw new InvalidOperationException(
                    $"{GetType().Name}: Unable to ascertain if buffer has been played back");
            }
#else
            if ((propInfo = typeof(T)
                    .GetProperty("PendingBuffers", BindingFlags.NonPublic | BindingFlags.Instance)) is null ||
                !(pendingBuffersList = (NativeList<EntityCommandBuffer>)propInfo.GetValue(bufferSystem)).IsCreated
            )
                throw new InvalidOperationException($"{GetType().Name}: Unable to ascertain if buffer has been played back");
#endif
        }

        public void AddJobDependency(JobHandle jobHandle)
        {
            bufferSystem.AddJobHandleForProducer(jobHandle);
        }

        public Action NewBuffer { get; set; }
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

    public class BeginSimCommandBuffer : CommandBuffer<BeginSimulationEntityCommandBufferSystem>
    {
        public BeginSimCommandBuffer(World world) : base(world)
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