using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;

namespace TimeManager
{
    public static class Data
    {
        public static List<(List<DayOfWeek> days, DateTime start, DateTime finish, TimeItems timeItems)> ItemsPatterns { get; set; }
        public static Dictionary<DateTime, TimeItems> Schedule;

        public static TimeItems GetTimeItems(this Dictionary<DateTime, TimeItems> Schedule, DateTime key)
        {
            if(Schedule.ContainsKey(key))
                return Schedule[key];
            try
            {
                foreach (var pattern in ItemsPatterns)
                {
                    if ((pattern.start != DateTime.MinValue && pattern.finish != DateTime.MinValue) &&
                        (pattern.start.Year > key.Year || pattern.start.Month > key.Month || pattern.start.Day > key.Day ||
                        pattern.finish.Year < key.Year || pattern.finish.Month < key.Month || pattern.finish.Day < key.Day))
                        continue;
                    if (pattern.days.Contains(key.DayOfWeek))
                    {
                        var newTimeItems = pattern.timeItems.Copy();
                        Schedule.Add(key, newTimeItems);
                        return newTimeItems;
                    }
                }
            }
            catch{ }
            var emptyTimeItems = new TimeItems();
            Schedule.Add(key, emptyTimeItems);
            return emptyTimeItems;
        }        
    }

    [Serializable]
    public class TimeItems : IEnumerable<TimeItem>, INotifyCollectionChanged
    {
        private static int lastIdentifier;
        //поля 
        private List<TimeItem> list { get; set; }
        //конструктор
        public TimeItems(params TimeItem[] _array)
        {
            lastIdentifier = 0;
            list = new List<TimeItem>();
            foreach (var item in _array)
            {
                Add(item);
            }
            OrderByTime();  //нужно исправить!!! не целесообразно!!!
        }

        //индексатор
        public TimeItem this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged(NotifyCollectionChangedAction action, object changedItem) =>
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));

        //методы
        public void Add(TimeItem item)
        {
            list.Add(item);
            item.Identifier = lastIdentifier++;
            item.Index = list.Count - 1;
            OnCollectionChanged(NotifyCollectionChangedAction.Add, this);
        }
        public void Clear() => list.Clear();
        public TimeItems Copy()
        {
            var NewtimeItems = new TimeItems();
            foreach (var item in list)
            {
                var newItem = new TimeItem();
                newItem.Finish = item.Finish;
                newItem.Identifier = item.Identifier;
                newItem.Index = item.Index;
                newItem.Name = item.Name;
                newItem.Note = item.Note;
                newItem.Start = item.Start;
                foreach(var note in item.Notes)
                {
                    newItem.Notes.Add((note.Note, note.IsCompleted));
                }
                NewtimeItems.Add(newItem);
            }
            return NewtimeItems;
        }
        public bool Remove(TimeItem item) => list.Remove(item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void DeleteByIdentifier(int identifier) 
        {
            var item = FindItemByIdentifier(identifier);
            list.Remove(item);
            int i = item.Index;
            for (; i<list.Count;i++)
            {
                list[i].Index = i;
            }
            
        }
        public int Count() => list.Count();
        public void OrderByString()
        {
            var tempRes = this.OrderBy(T => T.ToString());
            List<TimeItem> result = new List<TimeItem>();
            foreach (var item in tempRes)
            {
                result.Add(item);
            }
            list = result;
        }
        public void OrderByTime()
        { 
            var tempRes = this.OrderBy(T => T.Start);
            List<TimeItem> result = new List<TimeItem>();
            int i = 0;
            foreach (var item in tempRes)
            {
                result.Add(item);
                item.Index = i;
                i++;
            }
            list = result;
        }
        public TimeItems Search(string name)
        {
            var tempRes = this.Where(T => T.ToString().IndexOf(name) > -1);
            TimeItems result = new TimeItems();
            if (tempRes.Count() == 0)
                return result;
            foreach (var item in tempRes)
            {
                result.Add(item);
            }
            return result;
        }
        public TimeItem FindItemByName(string name)
        {
            var tempRes = this.Where(T => T.Name == name);
            TimeItems result = new TimeItems();
            if (tempRes.Count() == 0)
                return null;
            TimeItem timeItem = new TimeItem();
            foreach (var item in tempRes)
            {
                timeItem = item;
            }
            return timeItem;
        }
        public TimeItem FindItemByIdentifier(int identifier)
        {
            var tempRes = this.Where(T => T.Identifier == identifier);
            TimeItems result = new TimeItems();
            if (tempRes.Count() == 0)
                return null;
            TimeItem timeItem = new TimeItem();
            foreach (var item in tempRes)
            {
                timeItem = item;
            }
            return timeItem;
        }

        //реализация интерфейса IEnumerable<T>
        public IEnumerator<TimeItem> GetEnumerator() => (new CollectionEnum(this));
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //класс - перечислитель
        private class CollectionEnum : IEnumerator<TimeItem>
        {
            //поля
            private readonly TimeItems _this;
            private int position = -1;

            //конструктор
            public CollectionEnum(TimeItems coll)
            {
                _this = coll;
            }

            //реализация интерфейса IEnumerator<T>
            public TimeItem Current => _this.list[position];

            TimeItem IEnumerator<TimeItem>.Current => _this.list[position];

            object IEnumerator.Current => _this.list[position];

            void IDisposable.Dispose()  //??
            {
                ((IEnumerator)this).Reset();
            }
            bool IEnumerator.MoveNext()
            {
                position++;
                return (position < _this.list.Count());
            }
            void IEnumerator.Reset() => position = -1;

        }
    }

    [Serializable]
    public class TimeItem : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public int Identifier { get; set; }
        public string Name { get; set; }
        public List<(string Note, bool IsCompleted)> Notes { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Finish { get; set; }

        public string Note { get; set; }
        public TimeItem()
        {
            Index = 0;
            Identifier = 0;
            Start = new TimeSpan();
            Finish = new TimeSpan();
            Name = "Название";
            Notes = new List<(string, bool)>();            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
