using NUnit.Framework;
using Unity.Entities;

namespace Sibz.CommandBufferHelpers.Tests
{
    public class CommandBufferTests
    {
        private static World TestWorld { get; set; }

        private BeginInitializationEntityCommandBufferSystem BufferSystem =>
            TestWorld.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

        private static EntityQuery GetSingletonQuery<T>()
            where T : struct, IComponentData => TestWorld.EntityManager.CreateEntityQuery(typeof(T));

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            new World("test must not remove");
        }

        [SetUp]
        public void SetUp()
        {
            TestWorld = new World("Test");
            TestWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            TestWorld.Dispose();
        }

        [Test]
        public void WhenBufferSystemDoesNotExist_ShouldThrow()
        {
            Assert.Catch(() => new EndInitCommandBuffer(TestWorld));
        }

        [Test]
        public void BufferedCommandShouldExecuteNextFrame()
        {
            BeginInitCommandBuffer b = new BeginInitCommandBuffer(TestWorld);
            b.Buffer.AddComponent<TestComponent>(b.Buffer.CreateEntity());
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestComponent>().CalculateEntityCount());
        }

        [Test]
        public void ShouldRunOverMultipleFramesFine()
        {
            BeginInitCommandBuffer b = new BeginInitCommandBuffer(TestWorld);
            b.Buffer.AddComponent<TestComponent>(b.Buffer.CreateEntity());
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestComponent>().CalculateEntityCount());
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestComponent>().CalculateEntityCount());
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestComponent>().CalculateEntityCount());
            b.Buffer.AddComponent<TestComponent>(b.Buffer.CreateEntity());
            BufferSystem.Update();
            Assert.AreEqual(2, GetSingletonQuery<TestComponent>().CalculateEntityCount());
        }

        [Test]
        public void Extension_ShouldCreateSingleton()
        {
            BeginInitCommandBuffer b = new BeginInitCommandBuffer(TestWorld);
            b.CreateSingleton<TestComponent>();
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestComponent>().CalculateEntityCount());
        }
        [Test]
        public void Extension_ShouldCreateSingletonWithData()
        {
            BeginInitCommandBuffer b = new BeginInitCommandBuffer(TestWorld);
            b.CreateSingleton(new TestComponentWithData { Index =  5});
            BufferSystem.Update();
            Assert.AreEqual(1, GetSingletonQuery<TestComponentWithData>().CalculateEntityCount());
        }
        [Test]
        public void Extension_ShouldCreateSingletonWithCorrectData()
        {
            BeginInitCommandBuffer b = new BeginInitCommandBuffer(TestWorld);
            b.CreateSingleton(new TestComponentWithData { Index =  5});
            BufferSystem.Update();
            Assert.AreEqual(5, GetSingletonQuery<TestComponentWithData>().GetSingleton<TestComponentWithData>().Index);
        }
    }

    public struct TestComponent : IComponentData
    {
    }
    public struct TestComponentWithData : IComponentData
    {
        public int Index;
    }
}