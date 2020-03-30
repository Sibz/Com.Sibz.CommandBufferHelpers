using Unity.Entities;
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
            commandBuffer.IsCreated ? (commandBuffer = bufferSystem.CreateCommandBuffer()) : commandBuffer;

        public EntityCommandBuffer.Concurrent Concurrent =>
            Buffer.ToConcurrent();

        public CommandBuffer(World world)
        {
            bufferSystem = world.GetExistingSystem<T>();
        }
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