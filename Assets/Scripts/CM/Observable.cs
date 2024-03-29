using System;
using TriInspector;

[Serializable]
public class Observable<T>
{
    [ShowInInspector]
    private T _value;
    
    public Action<T> OnValueChanged;

    public Observable(T value)
    {
        _value = value;
    }
    
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }
}