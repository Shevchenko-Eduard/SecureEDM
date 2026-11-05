namespace SecureEDM.Client.Infrastructure;

public class CacheInMemoryTest
{
    [Fact]
    public async Task GlobalInitSetAndGet()
    {
        var p = new PoolCachInMemory();
        var c1 = new CachInMemory<Obj1, Obj2>(p);
        var c2 = new CachInMemory<Obj1, Obj2>(p);
        Obj1 key = new(1);
        Obj2 value = new(2);
        await c1.InitGlobal("test");
        await c2.InitGlobal("test");
        await c1.Set(key, value);
        var res = await c2.Get(key);
        Assert.Equal(value, res);
    }
    [Fact]
    public async Task LocalInitSetAndGet()
    {
        var p = new PoolCachInMemory();
        var c1 = new CachInMemory<Obj1, Obj2>(p);
        var c2 = new CachInMemory<Obj1, Obj2>(p);
        Obj1 key = new(1);
        Obj2 value = new(2);
        await c1.Init<Obj1>("test");
        await c2.Init<Obj2>("test");
        await c1.Set(key, value);
        var res = await c2.Exists(key);
        Assert.False(res);
    }
    [Fact]
    public async Task TestTimerWithDelete()
    {
        var p = new PoolCachInMemory();
        var c1 = new CachInMemory<Obj1, Obj2>(p);
        Obj1 key = new(1);
        Obj2 value = new(2);
        await c1.InitGlobal("test", new(0, 0, 1));
        await c1.Set(key, value);
        await Task.Delay(2000);
        var res = await c1.Exists(key);
        Assert.False(res);
    }
    [Fact]
    public async Task TestTimer2WithDelete()
    {
        var p = new PoolCachInMemory();
        var c1 = new CachInMemory<Obj1, Obj2>(p);
        Obj1 key = new(1);
        Obj2 value1 = new(2);
        Obj2 value2 = new(3);
        await c1.InitGlobal("test", new(0, 0, 1));
        await c1.Set(key, value1);
        await Task.Delay(700);
        await c1.Set(key, value2);
        await Task.Delay(500);
        var res = await c1.Exists(key);
        Assert.True(res);
    }
    private record Obj1(int Id);
    private record Obj2(int Id);
}