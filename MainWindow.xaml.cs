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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;

namespace StubMaker
{
    class Stubs
    {
        public string[] stubs { get; set; }
    }

    public class EntryFields : INotifyPropertyChanged
    {
        string cppfield;
        string hppfield;
        string authorfield;
        string breiffield;
        string namefield;
        string projectfield;

        public string CPPField
        {
            get {return cppfield; }
            set
            {
                if (value != cppfield)
                {
                    cppfield = value;
                    RaisePropertyChanged("CPPField");
                }
            }
        }
        public string HPPField
        {
            get { return hppfield; }
            set
            {
                if (value != hppfield)
                {
                    hppfield = value;
                    RaisePropertyChanged("HPPField");
                }
            }
        }

        public string AuthorField
        {
            get { return authorfield; }
            set
            {
                if (value != authorfield)
                {
                    authorfield = value;
                    RaisePropertyChanged("AuthorField");
                }
            }
        }

        public string BriefField
        {
            get { return breiffield; }
            set
            {
                if (value != breiffield)
                {
                    breiffield = value;
                    RaisePropertyChanged("BriefField");
                }
            }
        }

        public string NameField
        {
            get { return namefield; }
            set
            {
                if (value != namefield)
                {
                    namefield = value;
                    RaisePropertyChanged("NameField");
                }
            }
        }

        public string ProjectField
        {
            get { return projectfield; }
            set
            {
                if (value != projectfield)
                {
                    projectfield = value;
                    RaisePropertyChanged("ProjectField");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum FileMode
        {
            cpp,
            h
        }

        EntryFields entryfields;
        public MainWindow()
        {

            InitializeComponent();
            // Read the stubs files.
            InitializeEntryFields();

        }

        private void InitializeEntryFields()
        {
            entryfields = new EntryFields();
            authorField.DataContext = entryfields;
            projectNameField.DataContext = entryfields;
            nameField.DataContext = entryfields;
            briefField.DataContext = entryfields;

            ViewCPP.DataContext = entryfields;
            ViewH.DataContext = entryfields;
        }

        private void btnCreateStub(object sender, RoutedEventArgs e)
        {
            string thing = entryfields.AuthorField;
        }

        private void btnSaveFileAt_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                CreateFileLoc.Text = folder;

            }
        }

        private void btnOpenStubFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                stubFolder.Text = folder;

                // Read all the files in the directory
                var fileNames = Directory.GetFiles(folder);

                //  Process file names to get the name of the stubs
                var stubNames = process_files(fileNames);

                Stubs stub = new Stubs();
                stub.stubs = stubNames.ToArray();
                cmbStubs.DataContext = stub;
            }
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbStubs.SelectedItem == null)
                return;

            string stubTemplate = cmbStubs.SelectedItem.ToString();

            // Check if empty
            if(stubTemplate == String.Empty)
            {
                return;
            }

            // We want to load both the .c and the .h file of the given name

            // Get all the files in the stub folder
            var fileNames = Directory.GetFiles(stubFolder.Text);

            foreach (string fileName in fileNames)
            {
                if(fileName.Contains(stubTemplate))
                {
                    // If its a .h
                    if(fileName.Contains(".h"))
                    {
                        // Read the file and set to the string
                        StreamReader reader = new StreamReader(fileName);
                        try
                        {
                            entryfields.HPPField = reader.ReadToEnd();
                        }
                        catch
                        {
                            Console.WriteLine("Failed reading of file");
                        }

                        finally
                        {
                            reader.Close();
                        }
                    }
                    // Cpp
                    else if(fileName.Contains(".cpp"))
                    {
                        // Read the file and set to cpp
                        // Read the file and set to the string
                        StreamReader reader = new StreamReader(fileName);
                        try
                        {
                            entryfields.CPPField = reader.ReadToEnd();
                        }
                        catch
                        {
                            Console.WriteLine("Failed reading of file");
                        }

                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
        }

        // Takes in an of filenames (files in a directory), 
        // and if there is the stub word remove it and place into a new return unique array
        private HashSet<string> process_files(string [] fileNames)
        {
            HashSet<string> returnValue = new HashSet<string>();
            string stub = "stub";

            foreach(string fileName in fileNames)
            {
                string fileN = System.IO.Path.GetFileName(fileName);
                fileN = System.IO.Path.GetFileNameWithoutExtension(fileN);
                // Check if it has stub in it.
                if(fileN.Contains(stub) || fileN.Contains("Stub"))
                {
                    // Remove the stub word and add into return value
                    returnValue.Add(fileN);
                }
            }
            return returnValue;
        }

        private string UpdateFileHeader(string currentHeader, EntryFields entryfields, FileMode filemode)
        {
            string resultHeader;
            string fileextension;
            string signature = @"// -----------------------------------------------------------------------------
// @Project Name    {0}
// @filename        {1}.{2}
// @author	        {3}
// @date       	    {4}
// @brief           {5}
//
// Copyright � 2019 DigiPen, All rights reserved.
// -----------------------------------------------------------------------------";

            if (filemode == FileMode.cpp)
                fileextension = ".cpp";
            else
                fileextension = ".h";

            resultHeader = String.Format(signature, entryfields.ProjectField, entryfields.NameField, fileextension, entryfields.AuthorField, DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt"),
                entryfields.BriefField);

            return resultHeader;
        }
    }
}
