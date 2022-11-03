namespace Service;

public class Trigger : ICloneable
{
    public readonly string Name;
    public readonly string Text;
    public readonly string[] FileTypes;

    private readonly object _forLock = new object();
    private int _foundsCount;
    public int FoundsCount
    {
        get => _foundsCount;
        set
        {
            // компилятор ругается, лучше заблокировать заданный объект на время присваивания
            lock (_forLock)
            {
                _foundsCount = value;
            }
        }
    }

    public Trigger(string name, string text, string[] fileTypes)
    {
        Name = name;
        Text = text;
        FileTypes = fileTypes;
        FoundsCount = 0;
    }
    
    public object Clone()
    {
        return new Trigger(Name, Text, FileTypes);
    }
}