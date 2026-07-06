using System.Collections;

List<int> list = new List<int>() { 3, 2, 3, 5, 5};
SmartStack<int> ints = new SmartStack<int>(list);
ints.PushRange(list);
ints.Push(7);
ints.Push(8);
Console.WriteLine(ints.Peek());
Console.WriteLine(ints.Pop());
Console.WriteLine(ints.Peek());
Console.WriteLine(ints.Contains(2));
Console.WriteLine(ints.Contains(12));

public class SmartStack<T> : IEnumerable<T>
{
    private T[] _items;
    public int Copacity { get; set; }
    public int Count {  get; set; }

    public SmartStack()
    {
        Copacity = 4;
        _items = new T[Copacity];
    }
    public SmartStack(int stackSize)
    {
        Copacity = stackSize;
        _items = new T[Copacity];
    }
    public SmartStack(IEnumerable<T> collection)
    {
        Copacity = collection.Count();
        Count = Copacity;
        _items = new T[Copacity];
        int i = 0;
        foreach (T item in collection)
            _items[i++] = item;
    }

    public void Push(T item)
    {
        if (Count < Copacity)
            _items[Count++] = item;
        else
        {
            Copacity *= 2;
            Array.Resize(ref _items, Copacity);
            _items[Count++] = item;
        }
    }

    public void PushRange(IEnumerable<T> collection)
    {
        if (Count + collection.Count() < Copacity)
        {
            foreach (T item in collection)
                _items[Count++] = item;
        }
        else
        {
            Copacity += collection.Count();
            Array.Resize(ref _items, Copacity);
            foreach (T item in collection)
                _items[Count++] = item;
        }
    }

    public T Pop() 
    {
        if (Count == 0)
            throw new InvalidOperationException("Стек пуст");
        Count--;
        return _items[Count];
    }

    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException("Стек пуст");
        return _items[Count - 1];
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (Count >= 1)
            for (int i = Count - 1;  i >= 0; i--)
                yield return _items[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Length)
                throw new ArgumentOutOfRangeException("Неверный индекс");
            return _items[Count - index - 1];
        }
        set
        {
            if (index < 0 || index >= _items.Length)
                throw new ArgumentOutOfRangeException("Неверный индекс");
            _items[Count - index - 1] = value;
        }
    }
}