using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Krestik0
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> board;
        private bool isGameActive;
        private string status;
        private Random random;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            board = new ObservableCollection<string>(Enumerable.Repeat("", 9));
            isGameActive = true;
            status = "Крестики ходят первыми";
            random = new Random();
        }

        public ObservableCollection<string> Board
        {
            get { return board; }
            set
            {
                board = value;
                OnPropertyChanged(nameof(Board));
            }
        }

        public bool IsGameActive
        {
            get { return isGameActive; }
            set
            {
                isGameActive = value;
                OnPropertyChanged(nameof(IsGameActive));
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGameActive) return;

            Button button = (Button)sender;
            int index = Array.IndexOf(new Button[] { Button0, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8 }, button);

            if (Board[index] == "")
            {
                Board[index] = "X";
                CheckGameStatus();
                if (IsGameActive)
                {
                    MakeRobotMove();
                    CheckGameStatus();
                }
            }
        }

        private void MakeRobotMove()
        {
            int index;
            do
            {
                index = random.Next(9);
            } while (Board[index] != "");

            Board[index] = "O";
        }

        private void CheckGameStatus()
        {
            int[][] winningCombinations = new int[][]
            {
                new int[] {0, 1, 2},
                new int[] {3, 4, 5},
                new int[] {6, 7, 8},
                new int[] {0, 3, 6},
                new int[] {1, 4, 7},
                new int[] {2, 5, 8},
                new int[] {0, 4, 8},
                new int[] {2, 4, 6}
            };

            foreach (var combination in winningCombinations)
            {
                if (Board[combination[0]] != "" && Board[combination[0]] == Board[combination[1]] && Board[combination[0]] == Board[combination[2]])
                {
                    Status = Board[combination[0]] == "X" ? "Крестики победили!" : "Нолики победили!";
                    IsGameActive = false;
                    return;
                }
            }

            if (!Board.Any(cell => cell == ""))
            {
                Status = "Ничья!";
                IsGameActive = false;
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            Board = new ObservableCollection<string>(Enumerable.Repeat("", 9));
            IsGameActive = true;
            Status = "Крестики ходят первыми";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}