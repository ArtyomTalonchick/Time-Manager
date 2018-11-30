using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TimeManager
{
	public partial class App : Application
	{        
        private Dictionary<DateTime, TimeItems> Schedule { get; set; }
        private string pathSchedule { get; set; }
        public App ()
		{
			InitializeComponent();

            pathSchedule = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pathSchedule.dat");
            deserialize();
            //func();
            //serialize();

            MainPage = new MainPage(Schedule);
        }

        private void func()
        {
            var timeItems = new TimeItems();

            var timeItem = new TimeItem { Name = "Сон", Note = "Не переводи будильник" };
            timeItem.Start = new TimeSpan(23, 0, 0);
            timeItem.Finish = new TimeSpan(6, 0, 0);
            timeItems.Add(timeItem);

            timeItem = new TimeItem { Name = "Душ", Notes = new List<(string, bool)> { ("Почистить зубы", true), ("Помыться", false), ("Одеться", false), } };
            timeItem.Start = new TimeSpan(6, 0, 0);
            timeItem.Finish = new TimeSpan(6, 10, 0);
            timeItems.Add(timeItem);

            timeItem = new TimeItem { Name = "Дорога" };
            timeItem.Start = new TimeSpan(6, 45, 0);
            timeItem.Finish = new TimeSpan(7, 30, 1);
            timeItems.Add(timeItem);

            timeItem = new TimeItem { Name = "Завтрак", Note = "Завтрак - главный прием пищи" };
            timeItem.Start = new TimeSpan(6, 10, 0);
            timeItem.Finish = new TimeSpan(6, 30, 0);
            timeItems.Add(timeItem);
            
            Schedule = new Dictionary<DateTime, TimeItems>();
            Schedule.Add(new DateTime(2018, 12, 01), timeItems);
        }

        public void serialize()
        {
            var formatter = new BinaryFormatter();           
            using (Stream s = File.Create(pathSchedule))
                formatter.Serialize(s, Schedule);
        }
        public void deserialize()
        {
            var formatter = new BinaryFormatter();
            using (Stream s = File.OpenRead(pathSchedule))
                Schedule = (Dictionary<DateTime, TimeItems>)formatter.Deserialize(s);
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
            serialize();
        }

		protected override void OnResume ()
		{
            serialize();
        }
	}
}
