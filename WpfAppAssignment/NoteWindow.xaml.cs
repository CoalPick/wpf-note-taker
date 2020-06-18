using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfAppAssignment
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        // The ID of the note being edited
        public uint noteId;

        public NoteWindow() 
        {
            InitializeComponent();
        }

        // Function for clicking the Confirm Changes button
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Note noteChanges = new Note //TODO: Replace commas with &#44; entity
            {
                Title = TitleTextBox.Text,
                Text = TextBox.Text,
                Id = noteId,
                // The ?? syntax check if the left value is null and return the right value if so
                // Making sure that it sets HighPriority to false if HighPriorityCheckbox.IsChecked is (somehow) null 
                // as we can't implicitly convert from nullable bool to non-nullable bool
                HighPriority = HighPriorityCheckbox.IsChecked ?? false 
            };
            // Making sure there is a title so the user can click on the note to edit it
            if (noteChanges.Title == "")
            {
                noteChanges.Title = "(No Title)";
            }

            // Find and update the original note that is being edited, or add it if this is a new note
            int index = MainWindow.notesSource.FindIndex(x => x.Id == noteId);
            if (index != -1) // Existing note
            {
                noteChanges.Rank = MainWindow.notesSource[index].Rank;
                MainWindow.notesSource[index] = noteChanges;
            }
            else  // New note
            {
                noteChanges.Rank = 0;

                MainWindow.notesSource.Add(noteChanges);
            }

            // Close the window
            Close();

        }

        // Function for clicking the Remove button
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // Find and remove the original note, or just close the window
            int index = MainWindow.notesSource.FindIndex(x => x.Id == noteId);
            if (index != -1)
            {
                MainWindow.notesSource.RemoveAt(index);
            }

            // Close the window
            Close();
        }
    }
}
