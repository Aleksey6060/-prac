using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace DailyPlanner
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            listBox.ItemsSource = Notes;
        }



        private void CreateEntry_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Now;

            Note newNote = new Note
            {
                Title = title,
                Description = description,
                Date = selectedDate
            };

            Notes.Add(newNote);

            MessageBox.Show($"Запись создана:\nЗаголовок: {title}\nОписание: {description}\nДата: {selectedDate}");
        }



        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "notes.json");

            JsonSerializerHelper.Serialize(Notes, filePath);
            MessageBox.Show("Заметки сохранены на рабочем столе");
        }


        public class JsonSerializerHelper
        {
            public static void Serialize<T>(T obj, string fileName)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, fileName);

                string json = JsonSerializer.Serialize(obj);

                File.WriteAllText(filePath, json);
            }

            public static T Deserialize<T>(string fileName)
            {
                string json = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<T>(json);
            }
        }


        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                Note selectedNote = (Note)listBox.SelectedItem;
                Notes.Remove(selectedNote);
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }




        private void SaveNotes_Click(object sender, RoutedEventArgs e)
        {
            SaveNotesToJson("notes.json");
            MessageBox.Show("Заметки сохранены в файл notes.json");
        }

        private void SaveNotesToJson(string fileName)
        {
            string json = JsonSerializer.Serialize(Notes);
            File.WriteAllText(fileName, json);
        }



        public class Note
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }

            public override string ToString()
            {
                return $"{Title} - {Date.ToString("HH:mm dd/MM/yyyy")}";
            }
        }
    }

}