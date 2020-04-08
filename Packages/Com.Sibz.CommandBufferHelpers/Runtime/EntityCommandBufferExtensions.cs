using Unity.Entities;

namespace Sibz.CommandBufferHelpers
{
    public static class EntityCommandBufferExtensions
    {
        public static Entity CreateSingleton<T>(this ICommandBuffer commandBuffer, T data)
            where T : struct, IComponentData =>
            commandBuffer.Buffer.CreateSingleton(data);

        public static Entity CreateSingleton<T>(this ICommandBuffer commandBuffer)
            where T : struct, IComponentData =>
            commandBuffer.Buffer.CreateSingleton<T>();

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

        public static Entity CreateSingleton<T>(this EntityCommandBuffer.Concurrent commandBuffer, int index)
            where T : struct, IComponentData
        {
            Entity entity = commandBuffer.CreateEntity(index);
            commandBuffer.AddComponent<T>(index, entity);
            return entity;
        }

        public static Entity CreateSingleton<T>(this EntityCommandBuffer.Concurrent commandBuffer, int index, T data)
            where T : struct, IComponentData
        {
            Entity entity = CreateSingleton<T>(commandBuffer, index);
            commandBuffer.SetComponent(index, entity, data);
            return entity;
        }
    }
}