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
	public partial class AddDayPatternPage : ContentPage
    {
        private TimeItems timeItems { get; set; }
        private (List<DayOfWeek> days, DateTime start, DateTime finish, TimeItems timeItems) newPattern;
        private List<ItemsSource> choicePatternListView;

        //Коллекция TimeItem и визуальных элементов для их представления
        private List<(TimeItem timeItem, Grid grid, TimePicker startTimePicker, TimePicker finishTimePicker)> ListOfItemAndViews;

        //конструктор
        public AddDayPatternPage ()
		{
            InitializeComponent();
            newPattern.days = new List<DayOfWeek>();

            timeItems = new TimeItems();
            newPattern.start = DateTime.MinValue;
            newPattern.finish = DateTime.MinValue;
            ListOfItemAndViews = new List<(TimeItem, Grid, TimePicker, TimePicker)>();
            
            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12, GridUnitType.Star) });
            
            ChoicePattern();
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
                deleteButton.Clicked += (_s, _e) => deleteItemButton_Clicked(item.Identifier, copy_i);
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
            var addTimeItemButton = new Button { Text = "Добавить элемент", HorizontalOptions = LayoutOptions.Center };
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
        private void deleteItemButton_Clicked(int identifier, int index)    //ПЕРЕДЕЛАТЬ!!!
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
            if(!Data.ItemsPatterns.Contains(newPattern))
                Data.ItemsPatterns.Add(newPattern);
        }

        //обработчик кнопки сохранения шаблона 
        private void SaveButton_Clicked(object s, EventArgs e)
        {
            SaveChanges();
            ChoicePattern();
        }

        //обработчик кнопки удаления шаблона
        private void DeleteButton_Clicked(object s, EventArgs e)
        {
            if (Data.ItemsPatterns.Contains(newPattern))
                Data.ItemsPatterns.Remove(newPattern);
            ChoicePattern();
        }

        private void ChoicePattern()
        {
            choicePatternListView = new List<ItemsSource>();
            foreach (var pattern in Data.ItemsPatterns)
                choicePatternListView.Add(new ItemsSource { Pattern = pattern });

            var patternsListView = new ListView();
            patternsListView.ItemsSource = choicePatternListView;
            patternsListView.HasUnevenRows = true;
            patternsListView.ItemTemplate = new DataTemplate(() =>
            {
                Label daysLabel = new Label { FontSize = 16, TextColor = ColorSetting.colorOfName};
                daysLabel.SetBinding(Label.TextProperty, "Days");
                Label IntervalLabel = new Label { FontSize = 12, TextColor = Color.Default };
                IntervalLabel.SetBinding(Label.TextProperty, "Interval");
                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(0, 5),
                        Orientation = StackOrientation.Vertical,
                        Children = { daysLabel, IntervalLabel }
                    }
                };
            });
            patternsListView.ItemSelected += (s, e) =>
            {
                Content = GridOfTimeTable;
                newPattern = Data.ItemsPatterns[choicePatternListView.IndexOf((ItemsSource)patternsListView.SelectedItem)];
                timeItems = newPattern.timeItems;
                InitializeGridOfTimeItem();
            };
            Content = patternsListView;
        }

        private void BackButton_Clicked(object s, EventArgs e) => ChoicePattern();

        //класс для представления шаблонов в ListView
        public class ItemsSource
        {
            public (List<DayOfWeek> days, DateTime start, DateTime finish, TimeItems timeItems) Pattern { get; set; }
            private string days;
            public string Days
            {
                get
                {
                    days = "";
                    string[] daysOfWeek = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
                    for (int i = 0; i < 7; i++)
                    {
                        if (Pattern.days.Contains((DayOfWeek)((i + 1) % 7)))
                            days += daysOfWeek[i] + " ";
                    }
                    if (days == "")
                        days = "На каждый день";
                    return days;
                }
            }
            private string interval;
            public string Interval
            {
                get
                {
                    interval = "";                   
                    if (Pattern.start != DateTime.MinValue)
                    {
                        interval += "с " + Pattern.start.ToShortDateString() + " по " + Pattern.finish.ToShortDateString();
                    }
                    if (interval == "")
                        interval = "Без промежутка";
                    return interval;
                }
            }
        }
    }
}