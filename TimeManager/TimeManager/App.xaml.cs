using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TimeManager
{
	public partial class App : Application
	{
        private TimeItems timeItems { get; set; }

		public App ()
		{
			InitializeComponent();
            timeItems = new TimeItems
            {
                new TimeItem { Name = "Сон", Start = "23:00", Finish = "06:00", Note = "Не переводи будильник" },
                new TimeItem { Name = "Душ", Start = "06:00", Finish = "06:10" },
                new TimeItem { Name = "Завтрак", Start = "06:10", Finish = "06:30", Note = "Завтрак - главный прием пищи" },
                new TimeItem { Name = "Дорога", Start = "06/45", Finish = "07.30"},
            };

            MainPage = new TimeManager.MainPage(timeItems);
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
