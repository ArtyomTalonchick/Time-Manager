﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeManager
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        private Dictionary<DateTime, TimeItems> Schedule { get; set; }

        public MainPage(Dictionary<DateTime, TimeItems> _schedule)
        {
            InitializeComponent();
            Schedule = _schedule;

            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainPageMenuItem;
            var page = new Page();
            if (item == null)
                return;
            else if (item.Title == "Расписание")
            {
                page = new TimeTable(Schedule);
            }
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}