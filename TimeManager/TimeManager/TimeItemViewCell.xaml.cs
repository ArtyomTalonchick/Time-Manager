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
	public partial class TimeItemViewCell : ViewCell
    {
        public static readonly BindableProperty NameProperty = 
            BindableProperty.Create("Name", typeof(string), typeof(TimeItemViewCell), default(string));
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);            
        }
        public static readonly BindableProperty StartProperty =
           BindableProperty.Create("Start", typeof(TimeSpan), typeof(TimeItemViewCell), default(TimeSpan));
        public TimeSpan Start
        {
            get => (TimeSpan)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }
        public static readonly BindableProperty FinishProperty =
          BindableProperty.Create("Finish", typeof(TimeSpan), typeof(TimeItemViewCell), default(TimeSpan));
        public TimeSpan Finish
        {
            get => (TimeSpan)GetValue(FinishProperty);
            set => SetValue(FinishProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                nameLabel.Text = Name;
                finishLabel.Text = Finish.ToString(@"hh\:mm");
                startLabel.Text = Start.ToString(@"hh\:mm");
            }
        }
        
        public static readonly BindableProperty NotesProperty =
           BindableProperty.Create("Notes", typeof(List<(string Note, bool IsCompleted)>), typeof(TimeItemViewCell), default(List<(string Note, bool IsCompleted)>));
        public List<(string Note, bool IsCompleted)> Notes
        {
            get => (List<(string Note, bool IsCompleted)>)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public void ShowNotes()
        {
            List<(string Note, bool IsCompleted)> Note = new List<(string Note, bool IsCompleted)>
            {
                ("fdsfd", false),
                ("12", true),
                ("fd4324sfd", true),
                ("fds13212d", false),
                ("3243", true),
            };
            int i = 1;
            foreach (var note in Note) 
            {
                NameAndNotesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                NameAndNotesGrid.Children.Add(new NoteView { Text = note.Note, ValueOfSwitch = note.IsCompleted }, 0, i++);
                
            }
        }

        public TimeItemViewCell ()
		{
            InitializeComponent ();
		}

	}
}