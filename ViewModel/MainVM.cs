using Fill.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fill.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class MainVM
    {
        Manager model;
        public ObservableCollection<Item> Items { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
        public bool IsRunning { get; set; }

        public IEnumerable<Item> DisplayItems { get; set; }
        public ObservableCollection<List<Item>> FoundList { get; } = new ObservableCollection<List<Item>>();

       // [PropertyChanged.OnChangedMethod( "OnFoundPointerChanged" )]
        public int FoundPointer { get; set; }
        public int DisplayPointer { get; set; }

        public RelayCommand RunComm { get; set; }
        public RelayCommand ClearComm { get; set; }
        public RelayCommand PrevComm { get; set; }
        public RelayCommand NextComm { get; set; }
        public RelayCommand BreakComm { get; set; }


        public MainVM( Manager m )
        {
            model = m;
            FromModel();
            model.Filler.ResultEvent += ResultListener;
            model.Filler.FillerEvent += FillerListener;
            //runExe();

            RunComm = new RelayCommand( runExe, () => !IsRunning );
            ClearComm = new RelayCommand( clearExe, () => !IsRunning );
            PrevComm = new RelayCommand( prevExe, () => FoundPointer > 0 );
            NextComm = new RelayCommand( nextExe, () => FoundPointer < FoundList.Count - 1 );
            BreakComm = new RelayCommand( breakExe, () => IsRunning );
        }

        private void breakExe()
        {
            model.Filler.IsStopped = true;
        }
        private void prevExe()
        {
            if (FoundPointer > 0)
            {
                FoundPointer--;
                DisplayItems = FoundList[FoundPointer];
            }
        }
        private void nextExe()
        {
            if (FoundPointer < FoundList.Count - 1)
            {
                FoundPointer++;
                DisplayItems = FoundList[FoundPointer];
            }

        }
        void OnFoundPointerChanged()
        {
            if (FoundList.Count == 0)
                DisplayPointer = 0;
            else
                DisplayPointer = FoundPointer + 1;
        }

        private void clearExe()
        {
            DisplayItems = null;
            FoundList.Clear();
            FoundPointer = -1;
            Message = null;
            CommandManager.InvalidateRequerySuggested();
        }

        private void ResultListener( List<Item> list )
        {
            if (list == null)
                IsRunning = false;
            else
            {
                FoundList.Add( list );
                if (FoundList.Count == 1)
                    nextExe();
            }
            App.Current.Dispatcher.Invoke( () =>
            {
                CommandManager.InvalidateRequerySuggested();
            } );
        }

        void FillerListener( string msg )
        {
            Message = msg;
        }

        private void runExe()
        {
            IsRunning = true;
            clearExe();
            ToModel();
            model.Run();
        }

        public void ToModel()
        {
            model.Filler.Width = Width;
            model.Filler.Height = Height;
            model.Filler.ItemList = new List<Item>( Items );
        }
        public void FromModel()
        {
            Width = model.Filler.Width;
            Height = model.Filler.Height;
            Items = new ObservableCollection<Item>( model.Filler.ItemList );
        }
        public void Close()
        {
            ToModel();
            model.Filler.FillerEvent -= FillerListener;
            model.Filler.ResultEvent -= ResultListener;
        }
    }
}
