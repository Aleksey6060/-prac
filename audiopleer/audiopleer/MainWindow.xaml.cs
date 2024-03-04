using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Threading;



namespace audiopleer
{
    public partial class MainWindow : Window
    {
        private List<string> playlist;
        private DispatcherTimer positionUpdateTimer;
        public MainWindow()
        {
            InitializeComponent();
            playlist = new List<string>();

            PositionSlider.ValueChanged += Slider_ValueChanged;
            VolumeSlider.ValueChanged += Slider_ValueChanged;
            media.MediaEnded += Media_MediaEnded;
            media.MediaOpened += Media_MediaOpened;
            positionUpdateTimer = new DispatcherTimer();
            positionUpdateTimer.Interval = TimeSpan.FromMilliseconds(500); // Обновление каждые 500 миллисекунд
            positionUpdateTimer.Tick += PositionUpdateTimer_Tick;
            positionUpdateTimer.Start();
        }

        private int currentIndex = 0;
        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Проверяем, что текущий индекс не выходит за пределы плейлиста
            if (currentIndex < playlist.Count - 1)
            {
                currentIndex++; // Переходим к следующему файлу
                PlayAudio(currentIndex); // Воспроизводим следующий файл
            }

            if (isRepeatEnabled)
            {
                media.Position = TimeSpan.Zero; // Перематываем на начало
                media.Play(); // Воспроизводим снова
            }
            else
            {
                // Просто заканчиваем воспроизведение
                media.Stop();
            }
        }

        

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            PositionSlider.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            VolumeSlider.Value = media.Volume * 100.0;
        }

        private void openBTN_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog(); // Открываем диалоговое окно

            if (result == CommonFileDialogResult.Ok)
            {
                string[] audioExtensions = { ".mp3", ".m4a", ".wav", ".ogg", ".flac" }; 
                List<string> audioFiles = new List<string>();

                foreach (string extension in audioExtensions)
                {
                    audioFiles.AddRange(Directory.EnumerateFiles(dialog.FileName, "*" + extension, SearchOption.TopDirectoryOnly));
                }

                playlist.Clear(); // Очищаем текущий плейлист
                foreach (string file in audioFiles) // Перебираем аудиофайлы
                {
                    playlist.Add(file); // Добавляем файл в плейлист
                }
                PlaylistListBox.ItemsSource = playlist.Select(Path.GetFileName); // Обновляем источник данных для PlaylistListBox

                // Проверяем, что плейлист не пуст и воспроизводим первый файл
                if (playlist.Count > 0)
                {
                    PlayAudio(0); // Воспроизводим первый файл в плейлисте
                }
            }
        }

        private void PlayAudio(int index)
        {
            if (index >= 0 && index < playlist.Count)
            {
                media.Source = new Uri(playlist[index]);
                media.Play();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender == PositionSlider)
            {
               
            }
            else if (sender == VolumeSlider)
            {
                
            }
        }

        private bool isPlaying = false;
        private TimeSpan lastPosition = TimeSpan.Zero;

        

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Count > 0 && currentIndex >= 0 && currentIndex < playlist.Count)
            {
                if (isPlaying)
                {
                    media.Pause();
                    isPlaying = false;
                    PlayPauseButton.Content = "Продолжить";
                }
                else
                {
                    media.Play();
                    isPlaying = true;
                    PlayPauseButton.Content = "Пауза";
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Count > 0)
            {
                currentIndex++;
                if (currentIndex >= playlist.Count)
                {
                    currentIndex = 0; // Переходим к началу плейлиста, если достигли конца
                }
                media.Source = new Uri(playlist[currentIndex]);
                media.Play();
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Count > 0)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = playlist.Count - 1; // Переходим к концу плейлиста, если достигли начала
                }
                media.Source = new Uri(playlist[currentIndex]);
                media.Play();
            }
        }

        private bool isRepeatEnabled = false;

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            isRepeatEnabled = !isRepeatEnabled; // Переключаем режим повтора
            RepeatButton.Content = isRepeatEnabled ? "Repeat On" : "Repeat Off"; // Изменяем текст кнопки
        }

        private bool isShuffleEnabled = false;
        private List<string> originalPlaylist; // Для хранения исходного порядка плейлиста

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            isShuffleEnabled = !isShuffleEnabled; // Переключаем режим перемешивания
            ShuffleButton.Content = isShuffleEnabled ? "Shuffle On" : "Shuffle Off"; // Изменяем текст кнопки

            if (isShuffleEnabled)
            {
                originalPlaylist = new List<string>(playlist); // Сохраняем исходный порядок
                ShufflePlaylist(); // Перемешиваем плейлист
            }
            else
            {
                playlist = new List<string>(originalPlaylist); // Возвращаем исходный порядок
                PlaylistListBox.ItemsSource = playlist.Select(Path.GetFileName); // Обновляем источник данных для PlaylistListBox
            }
        }

        private void ShufflePlaylist()
        {
            Random rng = new Random();
            int n = playlist.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = playlist[k];
                playlist[k] = playlist[n];
                playlist[n] = value;
            }
            PlaylistListBox.ItemsSource = playlist.Select(Path.GetFileName); // Обновляем источник данных для PlaylistListBox
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данная функция станет доступна в версии 1.1 ;)", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void PositionUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (media.NaturalDuration.HasTimeSpan)
            {
                // Обновление позиции слайдера
                PositionSlider.Value = media.Position.TotalSeconds;

                // Обновление текстовых показателей
                currentTimeLabel.Content = media.Position.ToString(@"m\:ss");
                remainingTimeLabel.Content = "-" + (media.NaturalDuration.TimeSpan - media.Position).ToString(@"m\:ss");
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (media.NaturalDuration.HasTimeSpan)
            {
                // Изменение текущей позиции аудиозаписи при перемещении слайдера
                TimeSpan newPosition = TimeSpan.FromSeconds(PositionSlider.Value);
                media.Position = newPosition;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Volume = VolumeSlider.Value / 100.0; // Значение слайдера в диапазоне от 0 до 100
        }

    }
}