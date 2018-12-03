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
        private string pathSchedule { get; set; }
        private string pathPatterns { get; set; }

        public App ()
		{
			InitializeComponent();
            InitializeColor();

            creatingPaths();
            deserialize();
            //func();
            //serialize();

        //    Data.ItemsPatterns = new List<(List<DayOfWeek> days, DateTime start, DateTime finish, TimeItems timeItems)>();

            MainPage = new MainPage();
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

            Data.Schedule = new Dictionary<DateTime, TimeItems>();
            Data.Schedule.Add(new DateTime(2018, 12, 01), timeItems);
        }
        public void creatingPaths()
        {
            pathSchedule = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pathSchedule.dat");
            pathPatterns = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pathPatterns.dat");

        }
        public void serialize()
        {
            var formatter = new BinaryFormatter();
            using (Stream s = File.Create(pathSchedule))
                formatter.Serialize(s, Data.Schedule);
            using (Stream s = File.Create(pathPatterns))
                formatter.Serialize(s, Data.ItemsPatterns);
        }
        public void deserialize()
        {
            var formatter = new BinaryFormatter();
            using (Stream s = File.OpenRead(pathSchedule))
                Data.Schedule = (Dictionary<DateTime, TimeItems>)formatter.Deserialize(s);
            using (Stream s = File.OpenRead(pathPatterns))
                Data.ItemsPatterns = (List<(List<DayOfWeek> days, DateTime start, DateTime finish, TimeItems timeItems)>)formatter.Deserialize(s);
        }

        //метод для инициализации цветов
        private void InitializeColor()
        {
            ColorSetting.colorOfStart = Color.FromHex("F44336");
            ColorSetting.colorOfFinish = Color.FromHex("42A5F5");
            ColorSetting.colorOfName = Color.FromHex("424242");
            ColorSetting.colorOfBox = Color.FromHex("FAFAFA");      //почти белый
            ColorSetting.colorOfPage = Color.FromHex("EEEEEE");     //серый
            ColorSetting.colorOfMaster = Color.FromHex("E3F2FD");
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
