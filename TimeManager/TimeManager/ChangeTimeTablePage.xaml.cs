using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace TimeManager
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangeTimeTablePage : ContentPage
    {
        private TimeItems timeItems { get; set; }

        public ChangeTimeTablePage (TimeItems _timeItems)
		{
			InitializeComponent ();
            timeItems = _timeItems;

            var nameEntrys = new List<Entry>();
            var startTimePickers = new List<TimePicker>();
            var finishTimePickers = new List<TimePicker>();
            
            var root = new TableRoot();
            foreach(var item in timeItems)
            {
                var nameGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength (3, GridUnitType.Star) },
                    }                 
                };
                var startGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength (3, GridUnitType.Star) },
                    }
                };
                var finishGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength (2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength (3, GridUnitType.Star) },
                    }
                };

                root.Add
                (
                    new TableSection(item.Name)
                    {
                        new ViewCell {View = nameGrid},
                        new ViewCell {View = startGrid},
                        new ViewCell {View = finishGrid},
                    }
                );
                nameEntrys.Add(new Entry { Text = item.Name });
                nameGrid.Children.Add(new Label { Text = "Название:" }, 0, 0);
                nameGrid.Children.Add(nameEntrys.Last(), 1, 0);
                startTimePickers.Add(new TimePicker { Time = new TimeSpan(item.start_int / 60, item.start_int % 60, 0) });
                startGrid.Children.Add(new Label { Text = "Время начала:" }, 0, 0);
                startGrid.Children.Add(startTimePickers.Last(), 1, 0);
                finishTimePickers.Add(new TimePicker { Time = new TimeSpan(item.finish_int / 60, item.finish_int % 60, 0) });
                finishGrid.Children.Add(new Label { Text = "Время окончания:" }, 0, 0);
                finishGrid.Children.Add(finishTimePickers.Last(), 1, 0);
            }

            TableOfTimeItem.Root = root;

            scrollView();


        }

        private void scrollView()
        {
            var grid = new Grid();
            int i = 0;           
            foreach (var item in timeItems)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = 100 });
                grid.Children.Add(new Label { Text = item.Name }, 0, i);
                i++;
            }
            this.Content = new ScrollView { Content = grid };
        }
        

    }
}
 