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
    [Serializable]
    public class TimeItems : IEnumerable<TimeItem>, INotifyCollectionChanged
    {
        //поля 
        private List<TimeItem> list { get; set; }
        //конструктор
        public TimeItems(params TimeItem[] _array)
        {
            list = new List<TimeItem>();
            foreach (var item in _array)
            {
                list.Add(item);
            }
            OrderByTime();
        }

        //индексатор
        public TimeItem this[int i]
        {
            get
            {
                return list[i];
            }
            set
            {
                list[i] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged(NotifyCollectionChangedAction action, object changedItem) =>
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));
         
        //методы
        public void Add(TimeItem item)
        {
            list.Add(item);
            OrderByTime();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, this);
        }
        public void Clear() => list.Clear();
        public void CopyTo(TimeItem[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public bool Remove(TimeItem item) => list.Remove(item);
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
            var tempRes = this.OrderBy(T => T.start_int);
            List<TimeItem> result = new List<TimeItem>();
            foreach (var item in tempRes)
            {
                result.Add(item);
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
        public string Name { get; set; }
        public int start_int { get; private set; }
        public int finish_int { get; private set; }
        private DateTime start;
        private DateTime finish;
        public string Start
        {
            get
            {
                return start.ToShortTimeString();
            }
            set
            {
                Regex regex1 = new Regex(@"^?\d\d.\d\d$");
                if (regex1.IsMatch(value))
                {
                    start = new DateTime(1, 1, 1, Convert.ToInt32(value.Substring(0, 2)) % 24, Convert.ToInt32(value.Substring(3, 2)) % 60, 0);
                    start_int = start.Hour * 60 + start.Minute;
                }
            }
        }
        public string Finish
        {
            get
            {
                return finish.ToShortTimeString();
            }
            set
            {
                Regex regex1 = new Regex(@"^?\d\d.\d\d$");
                if (regex1.IsMatch(value))
                {
                    finish = new DateTime(1, 1, 1, Convert.ToInt32(value.Substring(0, 2)) % 24, Convert.ToInt32(value.Substring(3, 2)) % 60, 0);
                    finish_int = finish.Hour * 60 + finish.Minute;
                }
            }
        }
        public string Note { get; set; }
        public TimeItem()
        {
            start = new DateTime();
            finish = new DateTime();
            Name = "-";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
