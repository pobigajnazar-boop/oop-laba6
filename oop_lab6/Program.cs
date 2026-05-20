using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Базовий клас амуніції (з Лабораторної №5).
/// </summary>
public abstract class Ammunition : IEquatable<Ammunition>
{
    public string Name { get; }
    public double Weight { get; }
    public decimal Price { get; }

    protected Ammunition(string name, double weight, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Назва не може бути порожньою.");
        if (weight <= 0) throw new ArgumentOutOfRangeException(nameof(weight), "Вага має бути більшою за 0.");
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Ціна не може бути від'ємною.");

        Name = name;
        Weight = weight;
        Price = price;
    }

    public bool Equals(Ammunition other)
    {
        if (other == null) return false;
        return Name == other.Name && Weight == other.Weight && Price == other.Price;
    }

    public override bool Equals(object obj) => Equals(obj as Ammunition);
    public override int GetHashCode() => HashCode.Combine(Name, Weight, Price);
    public override string ToString() => $"{GetType().Name} '{Name}' (Вага: {Weight} кг, Ціна: {Price})";
}

public class Helmet : Ammunition
{
    public int DefenseLevel { get; }
    public Helmet(string name, double weight, decimal price, int defenseLevel) : base(name, weight, price) => DefenseLevel = defenseLevel;
}

public class Armor : Ammunition
{
    public string Material { get; }
    public Armor(string name, double weight, decimal price, string material) : base(name, weight, price) => Material = material;
}

public class Weapon : Ammunition
{
    public int Damage { get; }
    public Weapon(string name, double weight, decimal price, int damage) : base(name, weight, price) => Damage = damage;
}

/// <summary>
/// Кастомна типізована колекція, що реалізує інтерфейс Set (ISet в C#).
/// Внутрішня структура: масив, початкова місткість 15, збільшення на 30%.
/// </summary>
public class CustomSet<T> : ISet<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;
    public bool IsReadOnly => false;
    public int Capacity => _items.Length;

    /// <summary>
    /// Конструктор 1: Порожній конструктор. Ініціалізує масив на 15 елементів.
    /// </summary>
    public CustomSet()
    {
        _items = new T[15];
        _count = 0;
    }

    /// <summary>
    /// Конструктор 2: Конструктор з одним об'єктом.
    /// </summary>
    public CustomSet(T initialItem) : this()
    {
        Add(initialItem);
    }

    /// <summary>
    /// Конструктор 3: Конструктор, який приймає стандартну колекцію.
    /// </summary>
    public CustomSet(IEnumerable<T> collection) : this()
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Додає елемент у множину. Перевіряє унікальність та збільшує масив на 30% за потреби.
    /// </summary>
    public bool Add(T item)
    {
        if (Contains(item)) return false;

        if (_count == _items.Length)
        {
            int newCapacity = _items.Length + (int)(_items.Length * 0.3);
            Array.Resize(ref _items, newCapacity);
        }

        _items[_count] = item;
        _count++;
        return true;
    }

    void ICollection<T>.Add(T item) => Add(item);

    public bool Contains(T item)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item)) return true;
        }
        return false;
    }

    public bool Remove(T item)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item))
            {
                _items[i] = _items[_count - 1];
                _items[_count - 1] = default;
                _count--;
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _count);
        _count = 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _count);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        foreach (var item in other) Add(item);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        var otherItems = other.ToList();
        for (int i = _count - 1; i >= 0; i--)
        {
            if (!otherItems.Contains(_items[i])) Remove(_items[i]);
        }
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        foreach (var item in other) Remove(item);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        foreach (var item in other)
        {
            if (Contains(item)) Remove(item);
            else Add(item);
        }
    }

    public bool IsSubsetOf(IEnumerable<T> other) => this.All(other.Contains);
    public bool IsSupersetOf(IEnumerable<T> other) => other.All(Contains);
    public bool IsProperSupersetOf(IEnumerable<T> other) => IsSupersetOf(other) && _count > other.Count();
    public bool IsProperSubsetOf(IEnumerable<T> other) => IsSubsetOf(other) && _count < other.Count();
    public bool Overlaps(IEnumerable<T> other) => other.Any(Contains);
    public bool SetEquals(IEnumerable<T> other) => IsSubsetOf(other) && IsSupersetOf(other);

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Виконавчий клас програми.
/// </summary>
public class Lab6
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            Console.WriteLine("--- Тестування Конструктора 1 (Порожній) ---");
            CustomSet<Ammunition> set1 = new CustomSet<Ammunition>();
            Console.WriteLine($"Початкова місткість масиву: {set1.Capacity}, Кількість елементів: {set1.Count}");

            Console.WriteLine("\n--- Тестування Конструктора 2 (Один об'єкт) ---");
            Ammunition sword = new Weapon("Екскалібур", 2.5, 1000m, 100);
            CustomSet<Ammunition> set2 = new CustomSet<Ammunition>(sword);
            Console.WriteLine($"Кількість елементів: {set2.Count}, Елемент: {set2.First().Name}");

            Console.WriteLine("\n--- Тестування Конструктора 3 (Зі стандартної колекції) ---");
            List<Ammunition> standardList = new List<Ammunition>
            {
                new Helmet("Лицарський шолом", 3.0, 150m, 20),
                new Armor("Сталева кіраса", 15.0, 500m, "Сталь"),
                new Weapon("Кинджал", 1.0, 50m, 15)
            };
            CustomSet<Ammunition> myAmmunitionSet = new CustomSet<Ammunition>(standardList);
            
            Console.WriteLine("Елементи у множині:");
            foreach (var item in myAmmunitionSet)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\n--- Тестування властивостей Set (Унікальність) ---");
            bool isAdded = myAmmunitionSet.Add(new Helmet("Лицарський шолом", 3.0, 150m, 20));
            Console.WriteLine($"Спроба додати дублікат шолома. Успішно? {isAdded}");
            Console.WriteLine($"Кількість елементів після спроби: {myAmmunitionSet.Count}");

            Console.WriteLine("\n--- Тестування збільшення місткості на 30% ---");
            Console.WriteLine($"Поточна місткість: {myAmmunitionSet.Capacity}");
            for (int i = 0; i < 15; i++)
            {
                myAmmunitionSet.Add(new Weapon($"Меч клонів {i}", 1.0, 10m, 5));
            }
            Console.WriteLine($"Після додавання 15 елементів. Нова кількість: {myAmmunitionSet.Count}");
            Console.WriteLine($"Нова місткість масиву (було 15, стало 15 + 30%): {myAmmunitionSet.Capacity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }
}