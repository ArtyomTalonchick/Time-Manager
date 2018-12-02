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
	public partial class AddDayPatternPage : ContentPage
    {
        private TimeItems timeItems { get; set; }
        private (List<DayOfWeek> days, DateTime start, DateTime finish, TimeItems timeItems) newPattern;

        //Коллекция TimeItem и визуальных элементов для их представления
        private List<(TimeItem timeItem, Grid grid, TimePicker startTimePicker, TimePicker finishTimePicker)> ListOfItemAndViews;

        //конструктор
        public AddDayPatternPage ()
		{
			InitializeComponent ();
            newPattern.days = new List<DayOfWeek>();

            timeItems = new TimeItems();
            newPattern.start = DateTime.MinValue;
            newPattern.finish = DateTime.MinValue;
            ListOfItemAndViews = new List<(TimeItem, Grid, TimePicker, TimePicker)>();

            var saveTb = new ToolbarItem
            {
                Text = "Сохранить",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
            };
            saveTb.Clicked += (s, e) => SaveChanges();
            
            ToolbarItems.Add(saveTb);

            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12, GridUnitType.Star) });

            InitializeGridOfTimeItem();
        }

        //инициализирует GridOfTimeItem элементами шаблоного дня
        private void InitializeGridOfTimeItem()
        {           
            GridOfTimeItem.RowDefinitions.Clear();
            GridOfTimeItem.Children.Clear();
            ListOfItemAndViews.Clear();
            int i = 0;
            foreach (var item in timeItems)
            {
                var startEntry = new TimePicker { Time = item.Start, TextColor = ColorSetting.colorOfStart, VerticalOptions = LayoutOptions.End };
                var finishEntry = new TimePicker { Time = item.Finish, TextColor = ColorSetting.colorOfFinish, VerticalOptions = LayoutOptions.Start };
                var nameEntry = new Entry { Text = item.Name, TextColor = ColorSetting.colorOfName, FontSize = 30, FontAttributes = FontAttributes.Italic };
                var deleteButton = new Button { Text = "удалить", FontSize = 6, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.End };
                int copy_i = i;                                                                 //работает с помощью магии
                deleteButton.Clicked += (_s, _e) => deleteButton_Clicked(item.Identifier, copy_i);
                var nameGrid = new Grid
                {
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) } },
                };
                nameGrid.Children.Add(nameEntry, 0, 0);
                nameGrid.Children.Add(deleteButton, 1, 0);
                GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var gridForNotes = new Grid();
                ListOfItemAndViews.Add((item, gridForNotes, startEntry, finishEntry));
                gridForNotes.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var box = new BoxView { Color = ColorSetting.colorOfBox };
                GridOfTimeItem.Children.Add(box, 0, i);
                Grid.SetColumnSpan(box, 2);
                Grid.SetRowSpan(box, 2);
                GridOfTimeItem.Children.Add(startEntry, 0, i);
                GridOfTimeItem.Children.Add(finishEntry, 0, i + 1);
                gridForNotes.Children.Add(nameGrid, 0, 0);
                GridOfTimeItem.Children.Add(gridForNotes, 1, i);
                Grid.SetRowSpan(gridForNotes, 2);
                i += 2;
            }
            InitializeListOfGridForNotes();
            var addTimeItemButton = new Button { Text = "Добавить", HorizontalOptions = LayoutOptions.Center };
            addTimeItemButton.Clicked += addTimeItemButton_Clicked;
            GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            GridOfTimeItem.Children.Add(addTimeItemButton, 0, GridOfTimeItem.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(addTimeItemButton, 2);
        }

        //добавляет заметки к элементу дня
        private void InitializeListOfGridForNotes()
        {
            string defaultText = " Заметка  ";
            Color defaultColor = Color.FromHex("424242");
            int count = ListOfItemAndViews.Count;
            for (int i = 0; i < count; i++)
            {
                var viewForItem = ListOfItemAndViews[i];
                foreach (var note in viewForItem.timeItem.Notes)
                {
                    addEntry(note.Item1, Color.FromHex("000000"));
                }

                addEntry(defaultText, defaultColor);
                void addEntry(string _Text, Color _TextColor)
                {
                    viewForItem.grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var entry = new Entry { Text = _Text, TextColor = _TextColor, HorizontalOptions = LayoutOptions.Fill };
                    viewForItem.grid.Children.Add(entry, 0, viewForItem.grid.RowDefinitions.Count - 1);
                    entry.Focused += (s, e) =>
                    {
                        if (entry.Text == defaultText)
                        {
                            entry.Text = "";
                            entry.TextColor = Color.FromHex("000000");
                        }
                    };
                    entry.Unfocused += (s, e) =>
                    {
                        if (entry.Text == "")
                        {
                            entry.Text = defaultText;
                            if (((Entry)viewForItem.grid.Children.Last()).Text == defaultText
                                && (Entry)viewForItem.grid.Children.Last() != entry && viewForItem.grid.Children.Count > 2)
                            {

                                int countForEntry = viewForItem.grid.Children.Count;
                                for (int j = viewForItem.grid.Children.IndexOf(entry); j < countForEntry - 1; j++)
                                {
                                    ((Entry)viewForItem.grid.Children[j]).Text = ((Entry)viewForItem.grid.Children[j + 1]).Text;
                                }
                                viewForItem.grid.Children.RemoveAt(countForEntry - 1);
                                ((Entry)viewForItem.grid.Children[countForEntry - 2]).TextColor = defaultColor;
                            }
                        }
                        else
                        {
                            if (((Entry)viewForItem.grid.Children.Last()).Text != defaultText)
                            {
                                addEntry(defaultText, Color.FromHex("424242"));
                            }
                        }
                    };
                }
            }
        }

        //добавление нового элемента дня
        private void addTimeItemButton_Clicked(object s, EventArgs e)   //ПЕРЕДЕЛАТЬ!!!
        {
            SaveChanges();
            timeItems.Add(new TimeItem());
            InitializeGridOfTimeItem();
        }

        //удаление элемента дня
        private void deleteButton_Clicked(int identifier, int index)    //ПЕРЕДЕЛАТЬ!!!
        {
            SaveChanges();
            timeItems.DeleteByIdentifier(identifier);
            InitializeGridOfTimeItem();
        }

        //сохраняет изменения
        private void SaveChanges()
        {
            foreach (var item in ListOfItemAndViews)
            {
                item.timeItem.Start = item.startTimePicker.Time;
                item.timeItem.Finish = item.finishTimePicker.Time;
                item.timeItem.Name = ((Entry)((Grid)item.grid.Children[0]).Children[0]).Text;
                item.timeItem.Notes.Clear();
                int count = item.grid.Children.Count;
                for (int i = 1; i < count - 1; i++)
                {
                    item.timeItem.Notes.Add((((Entry)item.grid.Children[i]).Text, false));
                }
            }
            newPattern.timeItems = timeItems;
            Data.ItemsPatterns.Add(newPattern);
        }

        private void ChoiceOfDaysButton_Clicked(object s, EventArgs e)
        {            
            var checkGrid= new Grid {VerticalOptions=LayoutOptions.Center };
            checkGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            checkGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            string[] daysOfWeek = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
            for (int i = 0; i < 7; i++)
            {
                int j = i;                                                              //МАГИЯ VERSION 2.0
                var label = new Label { Text = daysOfWeek[i], FontSize = 16, TextColor = Color.Default };
                var switchForDay = new Switch { HorizontalOptions = LayoutOptions.End };
                if (newPattern.days.Contains((DayOfWeek)((j + 1) % 7)))
                {
                    switchForDay.IsToggled = true;
                    label.TextColor = Color.Black;
                    label.FontAttributes = FontAttributes.Bold;
                }
                switchForDay.Toggled += (_s, _e) =>
                {
                    if (switchForDay.IsToggled)
                    {
                        label.TextColor = Color.Black;
                        label.FontAttributes = FontAttributes.Bold;
                        newPattern.days.Add((DayOfWeek)((j + 1) % 7));
                    }
                    else
                    {
                        label.TextColor = Color.Default;
                        label.FontAttributes = FontAttributes.None;
                        newPattern.days.Remove((DayOfWeek)((j + 1) % 7));
                    }
                };
                checkGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                checkGrid.Children.Add(switchForDay, 0, i);
                checkGrid.Children.Add(label, 1, i);
            }

            #region Choice Day Interval 
            //грид для элементов для выбора промежутка
            var gridForDate = new Grid();
            gridForDate.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridForDate.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridForDate.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gridForDate.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            gridForDate.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gridForDate.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            // элементы для выбора промежутка
            var labelStart = new Label
            {
                Text = "c",
                FontSize = 16,
                TextColor = Color.Default,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            var labelFinish = new Label
            {
                Text = "по",
                TextColor = Color.Default,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            var datePickerStart = new DatePicker { IsEnabled = false };
            datePickerStart.DateSelected += (_s, _e) => { newPattern.start = datePickerStart.Date; };
            var datePickerFinish = new DatePicker { IsEnabled = false };
            datePickerFinish.DateSelected += (_s, _e) => { newPattern.finish = datePickerFinish.Date; };
            gridForDate.Children.Add(labelStart, 0, 1);
            gridForDate.Children.Add(datePickerStart, 1, 1);
            gridForDate.Children.Add(labelFinish, 2, 1);
            gridForDate.Children.Add(datePickerFinish, 3, 1);
            // элементы для выбора промежутка
            var switcherForInterval = new Switch { HorizontalOptions=LayoutOptions.End };
            switcherForInterval.Toggled += (_s, _e) =>
            {
                if (switcherForInterval.IsToggled)
                {
                    labelStart.TextColor = Color.Black;
                    labelFinish.TextColor = Color.Black;
                    datePickerStart.IsEnabled = true;
                    datePickerFinish.IsEnabled = true;
                    newPattern.start = datePickerStart.Date;
                    newPattern.finish = datePickerFinish.Date;
                }
                else
                {
                    labelStart.TextColor = Color.Default;
                    labelFinish.TextColor = Color.Default;
                    datePickerStart.IsEnabled = false;
                    datePickerFinish.IsEnabled = false;
                    newPattern.start = DateTime.MinValue;
                    newPattern.finish = DateTime.MinValue;
                }
            };
            if (newPattern.start != DateTime.MinValue)
            {
                datePickerStart.Date = newPattern.start;
                datePickerFinish.Date = newPattern.finish;
                switcherForInterval.IsToggled = true;
            }
            var gridForName = new Grid();
            gridForName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gridForName.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gridForName.Children.Add(new Label { Text = "Задать промежуток", FontSize = 16, TextColor = Color.Black }, 0, 0);
            gridForName.Children.Add(switcherForInterval, 1, 0);
            gridForDate.Children.Add(gridForName, 0, 0);
            Grid.SetColumnSpan(gridForName, 4);

            checkGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkGrid.Children.Add(gridForDate, 0, 7);
            Grid.SetColumnSpan(gridForDate, 2);
            #endregion

            //кнопка для возврата к элементам дня
            var exitButton = new Button { Text = "Сохранить", BackgroundColor=Color.Transparent };
            exitButton.Clicked += (_s, _e) => 
            {
                ChoiceDayButton.Text = "";
                string[] shortDaysOfWeek = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
                for (int i = 0; i < 7; i++)
                {
                    if (newPattern.days.Contains((DayOfWeek)((i + 1) % 7)))
                        ChoiceDayButton.Text += shortDaysOfWeek[i] + " ";
                }
                if (newPattern.start != DateTime.MinValue)
                {
                    ChoiceDayButton.Text += "(с " + newPattern.start.ToShortDateString() + " по " + newPattern.finish.ToShortDateString() + ")";
                }
                if (ChoiceDayButton.Text == "")
                    ChoiceDayButton.Text = "Выбрать дни";
                Content = GridOfTimeTable;
            };
            checkGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkGrid.Children.Add(exitButton, 0, checkGrid.RowDefinitions.Count);
            Grid.SetColumnSpan(exitButton, 2);
            Content = checkGrid;

        }

    }
}