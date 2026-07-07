using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace EQX.Core.Vision.Algorithms
{
    public class KeyTypeValue : ObservableObject
    {
        private string key;
        private object _value;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                OnPropertyChanged(nameof(Key));
            }
        }
        public Type Type { get; set; }
        public int Id { get; set; }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public KeyTypeValue(string key, Type type)
        {
            Key = key;
            Type = type;
            if (type == typeof(string))
            {
                Value = string.Empty;
            }
            else
            {
                Value = Activator.CreateInstance(type);
            }
            Id = GetHashCode();
        }

        [JsonConstructor]
        public KeyTypeValue(string key, Type type, int id)
        {
            Key = key;
            Type = type;
            Id = id;
        }
    }

    public interface IObjectCollection
    {
        EventHandler ObjectCollectionChanged { get; set; }
        object? this[string key] { get; set; }
        object? this[int key] { get; set; }

        ObservableCollection<KeyTypeValue>? Keys { get; set; }
    }
}