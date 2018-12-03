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
    public partial class TimeTable : ContentPage
    {
        private List<Grid> listOfGrid;      //Коллекция Grid'оф для элементов дня 
                                                //(лежит в главном Grid'е, включает в себя время и название элемента)
        private Grid selectedGrid;          //Выбранный Grid из коллекции Grid'оф
        private TimeItem selectedItem;      //Элемент дня, соответствующий выбранному Grid'у
        
        private TimeItems timeItems { get; set; }                       //расписание на выбранный день
        private bool addNoteIsOpen;                                     //флаг для работы кнопки addNoteButton в выбранном элементе

        //конструктор
        public TimeTable()
        {
            InitializeComponent();
            addNoteIsOpen = false;

            var changeTb = new ToolbarItem
            {
                Text = "Изменить",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
            };
            changeTb.Clicked += async (s, e) =>
            {
                // await Navigation.PushAsync(new ChangeTimeTablePage(this));
                await Navigation.PushAsync(new CreatingScheduleTabbedPage());
            };
            ToolbarItems.Add(changeTb);

            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            GridOfTimeItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });

            listOfGrid = new List<Grid>();
            InitializeGridOfTimeItem();
        }
        
        //инициализирует GridOfTimeItem элементами дня соответсвующего выбранной дате на DatePickerOfTimeTable
        private void InitializeGridOfTimeItem()
        {
            this.BackgroundColor = ColorSetting.colorOfPage;
            timeItems = Data.Schedule.GetTimeItems(DatePickerOfTimeTable.Date);
            GridOfTimeItem.RowDefinitions.Clear();
            GridOfTimeItem.Children.Clear();
            listOfGrid.Clear();
            int i = 0;
            foreach (var item in timeItems)
            {
                var startLabel = new Label { Text = item.Start.ToString(@"hh\:mm"), FontSize = 16, TextColor = ColorSetting.colorOfStart, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End };
                var finishLabel = new Label { Text = item.Finish.ToString(@"hh\:mm"), FontSize = 16, TextColor = ColorSetting.colorOfFinish, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start };
                var nameButton = new Button { Text = item.Name, TextColor = ColorSetting.colorOfName, FontSize = 30, FontAttributes = FontAttributes.Italic, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.CenterAndExpand };
                nameButton.Clicked += nameButton_Clicked;
                GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                GridOfTimeItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var gridForItem = new Grid();
                listOfGrid.Add(gridForItem);
                gridForItem.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var box = new BoxView {Color = ColorSetting.colorOfBox };
                GridOfTimeItem.Children.Add(box, 0, i);
                Grid.SetColumnSpan(box, 2);
                Grid.SetRowSpan(box, 2);
                GridOfTimeItem.Children.Add(startLabel, 0, i);
                GridOfTimeItem.Children.Add(finishLabel, 0, i + 1);
                gridForItem.Children.Add(nameButton, 0, 0);
                GridOfTimeItem.Children.Add(gridForItem, 1, i);
                Grid.SetRowSpan(gridForItem, 2);
                i += 2;
            }
        }

        //обработчик выбора элемента дня  -----  показывает заметки
        private void nameButton_Clicked(object s, EventArgs e)
        {
            //если "открыто окно" для добавления заметок
            if (addNoteIsOpen)
            {
                selectedGrid.Children.RemoveAt(selectedGrid.Children.Count - 1);
                addNoteIsOpen = false;
            }
            //сохраняем изменения для прошлого выбранного элемента
            if (selectedGrid != null)
            {
                selectedItem.Notes.Clear();
                int count = selectedGrid.Children.Count - 2;    //название и кнопка "добавить"
                for (int j = 0; j < count; j++)
                {
                    var noteView = (NoteView)(selectedGrid.Children[j + 1]);
                    selectedItem.Notes.Add((noteView.Text, noteView.ValueOfSwitch));
                }
                var btn = (Button)selectedGrid.Children[0];
                var rowDefinition = selectedGrid.RowDefinitions[0];
                selectedGrid.Children.Clear();
                selectedGrid.Children.Add(btn, 0, 0);
                selectedGrid.RowDefinitions.Clear();
                selectedGrid.RowDefinitions.Add(rowDefinition);
            }
            selectedItem = timeItems.FindItemByName(((Button)s).Text);
            //если выбран предыдущий элемент, то закрываем его
            if (selectedGrid == listOfGrid[selectedItem.Index])
            {
                selectedGrid = null;
                return;
            }
            selectedGrid = listOfGrid[selectedItem.Index];            
            //добавляем все заметки для выбранного элемента дня
            int i = 1;
            foreach (var note in selectedItem.Notes)
            {
                selectedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var noteView = new NoteView { Text = note.Item1, ValueOfSwitch = note.Item2};
                noteView.Clicked += (_s, _e) => deleteNote_Clicked(noteView, (Button)s);
                selectedGrid.Children.Add(noteView, 0, i++);
            }
            //добавляем кнопку "добавить заметку"
            selectedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var addNoteButton = new Button { Text = "Добавить", FontSize = 10, BackgroundColor=Color.Transparent, HorizontalOptions = LayoutOptions.Center };
            addNoteButton.Clicked += (_s, _e) => addNoteButton_Clicked(addNoteButton, ref i);
            selectedGrid.Children.Add(addNoteButton, 0, i++);
        }
        
        //обработчик удаления заметки
        private void deleteNote_Clicked(NoteView noteView, Button button)
        {
            if (addNoteIsOpen)
            {
                selectedGrid.Children.RemoveAt(selectedGrid.Children.Count - 1);
                addNoteIsOpen = false;
            }
            selectedGrid.Children.RemoveAt(selectedGrid.Children.IndexOf(noteView));
            nameButton_Clicked(button, new EventArgs());
            nameButton_Clicked(button, new EventArgs());
        }

        //обработчик добавления заметки
        private void addNoteButton_Clicked(Button addNoteButton, ref int i)
        {
            if(!addNoteIsOpen)
            {
                addNoteButton.Text = "Сохранить";
                selectedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var entry = new Entry();
                selectedGrid.Children.Add(entry, 0, i++);
            }
            else
            {
                addNoteButton.Text = "Добавить";
                var text = ((Entry)selectedGrid.Children[i - 1]).Text;
                selectedGrid.Children.RemoveAt(i - 1);
                selectedGrid.Children.RemoveAt(i - 2);
                selectedGrid.Children.Add(new NoteView { Text = text }, 0, i - 2);
                selectedGrid.Children.Add(addNoteButton, 0, i - 1);
            }
            addNoteIsOpen = !addNoteIsOpen;
        }

        //обработчик выбора даты
        private void DatePickerOfTimeTable_DateSelected(object s, EventArgs e) => InitializeGridOfTimeItem();

        //метод для обнавления расписания
        public void ScheduleUpdate() => InitializeGridOfTimeItem();
    }
}