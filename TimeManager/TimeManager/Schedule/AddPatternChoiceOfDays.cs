using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeManager
{
    public partial class AddDayPatternPage : ContentPage
    {
        private void ChoiceOfDaysButton_Clicked(object s, EventArgs e)
        {
            var checkGrid = new Grid { VerticalOptions = LayoutOptions.Center };
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
            var switcherForInterval = new Switch { HorizontalOptions = LayoutOptions.End };
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
            var exitButton = new Button { Text = "Сохранить", BackgroundColor = Color.Transparent };
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
