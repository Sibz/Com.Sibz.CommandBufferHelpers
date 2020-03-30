using Unity.Entities;

namespace Sibz.CommandBufferHelpers
{
    public static class EntityCommandBufferExtensions
    {
        public static Entity CreateSingleton<T>(this EntityCommandBuffer commandBuffer)
            where T : struct, IComponentData
        {
            Entity entity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent<T>(entity);
            return entity;
        }

        public static Entity CreateSingleton<T>(this EntityCommandBuffer commandBuffer, T data)
            where T : struct, IComponentData
        {
            Entity entity = CreateSingleton<T>(commandBuffer);
            commandBuffer.SetComponent(entity, data);
            return entity;
        }
    }
}