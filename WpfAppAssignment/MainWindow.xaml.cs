using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfAppAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The list of notes that the notes table uses as a data source
        public static List<Note> notesSource = new List<Note>();
        
        // Path for the notes path (using @ so I don't need to write slashes twice)
        // Currently using the user's documents folder
        string path = @"C:\Users\" + Environment.UserName + @"\Documents\Notes.csv";

        public MainWindow()
        {
            InitializeComponent();

            Window1.Visibility = Visibility.Collapsed; // Notes section
            Window2.Visibility = Visibility.Collapsed;
            Window3.Visibility = Visibility.Collapsed;

            //Console.WriteLine(userName);
            //Console.WriteLine(path);

            if (!File.Exists(path)) // If the file doesn't exist then create it
            {
                try
                {

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("#id,rank,text,title,highPriority"); // Layout of the file for reference
                    }
                }
                catch (Exception ex) // This will likely be an issue with not having permission to modify the directory we're trying to create the file in
                {
                    MessageBox.Show(ex.Message, ex.GetBaseException().ToString());
                    throw;
                }
            }

            // DEBUG: Write the contents of the file to the console
            //using (StreamReader sr = File.OpenText(path))
            //{
            //    string s = "";
            //    while ((s = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(s);
            //    }
            //}

            // Get each line in the file and then parse each section
            string[] lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!line.StartsWith("#")) // Checking that the line doesn't start with a #
                {
                    try
                    {
                        string[] elements = line.Split(',');
                        Note fileNote = new Note
                        {
                            Id = Convert.ToUInt32(elements[0]),
                            Rank = Convert.ToInt32(elements[1]),
                            Text = elements[2],
                            Title = elements[3],
                            HighPriority = Convert.ToBoolean(elements[4])
                        };
                        notesSource.Add(fileNote);
                    }
                    catch (Exception ex) // This will likely be an issue with the title or text having commas
                    {
                        MessageBox.Show(ex.Message, ex.GetBaseException().ToString());
                        throw;
                    }
                }
            }

            NotesTable.ItemsSource = notesSource;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick; // Runs timer_Tick every interval (not currently set)
            timer.Start();

        }

        // Function for clicking the Notes button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RefreshNotes();

            if (Window1.Visibility == Visibility.Visible)
            {
                Window1.Visibility = Visibility.Collapsed;
            }
            else
            {
                Window1.Visibility = Visibility.Visible;
            }
            Window2.Visibility = Visibility.Collapsed;
            Window3.Visibility = Visibility.Collapsed;
        }

        //TODO: Add extra productivity tools
        // Function for clicking Button2 (currently unused)
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Window2.Visibility == Visibility.Visible)
            {
                Window2.Visibility = Visibility.Collapsed;
            }
            else
            {
                Window2.Visibility = Visibility.Visible;
            }
            Window1.Visibility = Visibility.Collapsed;
            Window3.Visibility = Visibility.Collapsed;
        }

        // Function for clicking Button3 (currently unused)
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Window3.Visibility == Visibility.Visible)
            {
                Window3.Visibility = Visibility.Collapsed;
            }
            else
            {
                Window3.Visibility = Visibility.Visible;
            }
            Window1.Visibility = Visibility.Collapsed;
            Window2.Visibility = Visibility.Collapsed;
        }

        // When clicking a note's title in the notes section open a new window
        private void Title_Click(object sender, RoutedEventArgs e)
        {
            Note selectedNote = (Note)NotesTable.SelectedItem;

            NoteWindow newWindow = new NoteWindow
            {
                Visibility = Visibility.Visible,
                Title = selectedNote.Title
            };
            newWindow.TitleTextBox.Text = selectedNote.Title;
            newWindow.TextBox.Text = selectedNote.Text;
            newWindow.noteId = selectedNote.Id;
            newWindow.HighPriorityCheckbox.IsChecked = selectedNote.HighPriority;
        }

        // Continuously update the time label
        private void timer_Tick(object sender, EventArgs e)
        {
            string now = DateTime.Now.ToString("HH:mm:ss");

            TimeLabel.Content = now;
        }

        // Handles the behaviour for clicking the Add Note button
        private void Add_Note_Button_Click(object sender, RoutedEventArgs e)
        {
            uint noteCount = (uint)notesSource.Count();

            NoteWindow newNote = new NoteWindow
            {
                noteId = noteCount,
                Visibility = Visibility.Visible,
                Title = "New Note"
            };
        }

        // Refresh the notes table so it's up to date
        public void RefreshNotes()
        {
            NotesTable.Items.Refresh();
        }

        // Function for clicking the Refresh button
        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshNotes();
        }

        // Function for clicking the Help button
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage = "Click on the 'Notes' button to show the table of notes. Click 'Add Note' to open the new note window. " +
                "Click the title of an existing note in the table to edit or remove it.";

            // Using the format Text - Title - Button - Icon
            MessageBox.Show(helpMessage, "Main Screen - Help", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        // For writing any Note object to a file
        private void WriteToFile(Note n, string path)
        {
            try
            {
                string lineOut = "";

                lineOut += n.Id.ToString() + ",";
                lineOut += n.Rank.ToString() + ",";
                lineOut += n.Text + ",";
                lineOut += n.Title + ",";
                lineOut += n.HighPriority.ToString();

                using (StreamWriter sw = new StreamWriter(path, true)) // This true part avoids completely overwritting the file
                {
                    sw.WriteLine(lineOut);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetBaseException().ToString());
                throw;
            }
        }

        // Called when the main window is closed
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                File.WriteAllText(path, "#id,rank,text,title,highPriority\n"); // Needs work; this is to avoid duplicating the notes

                foreach (var note in notesSource)
                {
                    WriteToFile(note, path);
                }
            }
            catch (Exception ex) // This will likely be an issue with not having permission to modify the directory we're trying to create the file in
            {
                MessageBox.Show(ex.Message, ex.GetBaseException().ToString());
                throw;
            }
        }

    }

    // Class used by NotesTable and when passing data between MainWindow and a NoteWindow
    public class Note
    {
        private uint id;
        private int rank;
        private string text;
        private string title;
        private bool highPriority;

        // Reserved if there is a need to change the get/set behaviour, instead of relying on the auto-properties
        public uint Id { get => id; set => id = value; }
        public int Rank { get => rank; set => rank = value; }
        public string Text { get => text; set => text = value; }
        public string Title { get => title; set => title = value; }
        public bool HighPriority { get => highPriority; set => highPriority = value; }
    }
    

}
