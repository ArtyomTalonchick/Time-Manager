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
        private Dictionary<DateTime, TimeItems> Schedule;
        private TimeItems timeItems { get; set; }
        private List<Grid> listOfGrid;      //Коллекция Grid'оф для элементов дня 

        public ChangeTimeTablePage (Dictionary<DateTime, TimeItems> _schedule)
        {
            InitializeComponent();
            Schedule = _schedule;

            

            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12, GridUnitType.Star) });

            listOfGrid = new List<Grid>();
            InitializeGridOfTimeItem();
        }

        //инициализирует GridOfTimeItem элементами дня соответсвующего выбранной дате на DatePickerOfTimeTable
        private void InitializeGridOfTimeItem()
        {
            try
            {
                timeItems = Schedule[DatePickerOfTimeTable.Date];
            }
            catch
            {
                timeItems = new TimeItems();
            }
            GridOfTimeItem.RowDefinitions.Clear();
            GridOfTimeItem.Children.Clear();
            listOfGrid.Clear();
            int i = 0;
            foreach (var item in timeItems)
            {
                var startEntry = new TimePicker { Time = item.Start };
                var finishEntry = new TimePicker { Time = item.Finish };
                var nameEntry = new Entry { Text = item.Name, TextColor = Color.FromHex("424242"), FontSize = 30, FontAttributes = FontAttributes.Italic};
                GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                GridOfTimeItem.Children.Add(startEntry, 0, i);
                GridOfTimeItem.Children.Add(finishEntry, 0, i + 1);
                var gridForItem = new Grid();
                listOfGrid.Add(gridForItem);
                gridForItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                gridForItem.Children.Add(nameEntry, 0, 0);
                GridOfTimeItem.Children.Add(gridForItem, 1, i);
                Grid.SetRowSpan(gridForItem, 2);
                i += 2;
            }
        }


    }
}
 