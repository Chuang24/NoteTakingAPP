using NotesAPP.Model;
using NotesAPP.ViewModel.commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesAPP.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {

        private bool isEditing;

        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                OnPropertyChanged("IsEditing");
            }
        }

        public ObservableCollection<Notebook> Notebooks { get; set; }
        private Notebook selectedNotebook;


        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                ReadNotes();
            }
            
            
        }

        private Note note;

        public Note SelectedNote
        {
            get { return note; }
            set
            {
                note = value;
                selectedNoteChange(this,new EventArgs ());
            }
        }

        public ObservableCollection<Note> Notes { get; set; }

        public NewNotebookCommand NewNotebookCommand { get; set; }
        public NewNoteCommand NewNoteCommand { get; set; }
        public BeginEditeCommand beginEditeCommand { get; set; }
        public HasEditedCommand HasEditedCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler selectedNoteChange;



        public NotesVM()
        {
            IsEditing = false;

            NewNotebookCommand = new NewNotebookCommand(this);
            NewNoteCommand = new NewNoteCommand(this);
            beginEditeCommand = new BeginEditeCommand(this);
            HasEditedCommand = new HasEditedCommand(this);
            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();

            ReadNotebooks();
            ReadNotes();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void CreateNoteBook()
        {
            Notebook newNotebook = new Notebook() {
                Name = "New Notebook",
                UserId = int.Parse(App.UserID)
            };
            DatabaseHelper.Insert(newNotebook);
            ReadNotebooks();
        }

        public void CreateNote(int notebookID)
        {
            Note newNote = new Note()
            {
                NotebookId = notebookID,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Title = "New Note"
            };

            DatabaseHelper.Insert(newNote);
            ReadNotes();
        }

        public void ReadNotebooks()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(DatabaseHelper.dbFile))
            {
                var noteboooks = conn.Table<Notebook>().ToList();//cast it to a list 
                Notebooks.Clear();

                    foreach(var notebook in noteboooks)
                {
                    Notebooks.Add(notebook);
                }
            }
        }

        public void ReadNotes()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(DatabaseHelper.dbFile))
            {
                if (SelectedNotebook != null)
                {
                    var notes = conn.Table<Note>().Where(n => n.NotebookId == SelectedNotebook.Id).ToList();

                    Notes.Clear();
                    foreach (var note in notes)
                    {
                        Notes.Add(note);
                    }
                }
            }

        }


        public void startEditing()
        {
            IsEditing = true;
        }

        public void HasRenamed(Notebook notebook)
        {
            if (notebook != null)
            {
                DatabaseHelper.Update(notebook);
                IsEditing = false;
                ReadNotebooks();
            }
        }

        public void UpdateSelectedNode()
        {
            DatabaseHelper.Update(SelectedNote);
        }
    }

    

}
