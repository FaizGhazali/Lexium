using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lexium.MVVM.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private Dictionary<string, string> _wordObj = new Dictionary<string, string>();
        private ObservableCollection<string> _wordList = new ObservableCollection<string>();
        public List<string> Words { get; set; } = new List<string>();

        public Dictionary<string, string> WordObj
        {
            get => _wordObj;
            set
            {
                _wordObj = value;
                UpdateWordList();
                UpdateList();
                OnPropertyChanged(nameof(WordObj));
            }
        }

        public void GetDefination(string key)
        {
            try
            {
                DefinationValue = WordObj[key];
            }
            catch
            {
                DefinationValue = "";
            }
            
        }

        public ObservableCollection<string> WordList
        {
            get => _wordList;
            private set
            {
                _wordList = value;
                OnPropertyChanged(nameof(WordList));
            }
        }
        private string _definationValue;
        public string DefinationValue
        {
            get => _definationValue;
            private set
            {
                _definationValue = value;
                OnPropertyChanged(nameof(DefinationValue));
            }
        }
        public MainWindowVM()
        {
            // Initialize with sample data
            WordObj = new Dictionary<string, string>
        {
            { "test", "Test Value" },
            { "test one two three", "Test Phrase Value" }
        };
        }

        // Method to add new word to dictionary
        public void AddWord(string key, string value)
        {
            WordObj[key] = value;
            UpdateWordList();
        }

        // Method to remove word from dictionary
        public void RemoveWord(string key)
        {
            if (WordObj.ContainsKey(key))
            {
                WordObj.Remove(key);
                UpdateWordList();
            }
        }

        // Update WordList based on WordObj keys
        private void UpdateWordList()
        {
            Words.Clear();
            foreach (var key in WordObj.Keys)
            {
                Words.Add(key);
            }
            UpdateList();
        }
        private void UpdateList()
        {
            WordList.Clear();
            foreach (var key in WordObj.Keys)
            {
                WordList.Add(key);
            }
        }

       

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
