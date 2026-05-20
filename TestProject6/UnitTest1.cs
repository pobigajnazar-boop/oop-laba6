using System;
using System.Collections.Generic;
using Xunit;

/// <summary>
/// Набір тестів для перевірки кастомної колекції CustomSet.
/// </summary>
public class UnitTest1
{
    [Fact]
    public void Constructor_Empty_ShouldInitializeWithCapacity15()
    {
        CustomSet<Ammunition> set = new CustomSet<Ammunition>();
        
        Assert.Equal(0, set.Count);
        Assert.Equal(15, set.Capacity);
    }

    [Fact]
    public void Add_DuplicateElement_ShouldNotIncreaseCount()
    {
        CustomSet<Ammunition> set = new CustomSet<Ammunition>();
        var sword1 = new Weapon("Меч", 2.0, 100m, 10);
        var sword2 = new Weapon("Меч", 2.0, 100m, 10); 
        
        bool firstAdd = set.Add(sword1);
        bool secondAdd = set.Add(sword2);

        Assert.True(firstAdd);
        Assert.False(secondAdd);
        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Add_WhenCapacityReached_ShouldIncreaseBy30Percent()
    {
        CustomSet<int> set = new CustomSet<int>();
        Assert.Equal(15, set.Capacity);

        for (int i = 0; i < 16; i++)
        {
            set.Add(i);
        }

        Assert.Equal(16, set.Count);
        Assert.Equal(19, set.Capacity); 
    }

    [Fact]
    public void Remove_ExistingElement_ShouldDecreaseCount()
    {
        var helmet = new Helmet("Шолом", 1.0, 50m, 5);
        CustomSet<Ammunition> set = new CustomSet<Ammunition>(helmet);
        
        bool isRemoved = set.Remove(helmet);
        
        Assert.True(isRemoved);
        Assert.Equal(0, set.Count);
    }

    [Fact]
    public void UnionWith_ShouldCombineUniqueElements()
    {
        var set = new CustomSet<int> { 1, 2, 3 };
        var list = new List<int> { 3, 4, 5 };
        
        set.UnionWith(list);
        
        Assert.Equal(5, set.Count);
        Assert.True(set.Contains(4));
    }
}