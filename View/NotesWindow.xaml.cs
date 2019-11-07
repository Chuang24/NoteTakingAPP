using NotesAPP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace NotesAPP.View
{
    /// <summary>
    /// Interaction logic for NoteWindwo.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        SpeechRecognitionEngine recognizer;
        NotesVM viewModel;

        public NoteWindow()
        {
            InitializeComponent();

            viewModel = new NotesVM();
            container.DataContext = viewModel;
            viewModel.selectedNoteChange += viewmodel_selectedNoteChanged;

            var currentCulture = (from r in SpeechRecognitionEngine.InstalledRecognizers()
                                  where r.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                  select r).FirstOrDefault();
            recognizer = new SpeechRecognitionEngine(currentCulture);

            GrammarBuilder builder = new GrammarBuilder();
            builder.AppendDictation();
            Grammar grammar = new Grammar(builder);
            recognizer.LoadGrammar(grammar);
            //recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecgonized;

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontFamiltyComboBox.ItemsSource = fontFamilies;

            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 28, 48, 72 };
            fontSizeComboBox.ItemsSource = fontSizes;
        }

        private void viewmodel_selectedNoteChanged(object sender, EventArgs e)
        {
            ContentRichTextBox.Document.Blocks.Clear();
            if (!string.IsNullOrEmpty(viewModel.SelectedNote.Filelocation))
            {
                FileStream filestream = new FileStream(viewModel.SelectedNote.Filelocation, FileMode.Open);
                TextRange range = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
                range.Load(filestream, DataFormats.Rtf);
            }
        }

        //Adding check to UserID from App.xaml.cs to see if any userId exists
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (string.IsNullOrEmpty(App.UserID))
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
            }
        }


        private void Recognizer_SpeechRecgonized(object sender, SpeechRecognizedEventArgs e)
        {
            string recgonizedText = e.Result.Text;
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(recgonizedText)));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                recognizer.RecognizeAsyncStop();
            }
        }

        private void ContentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountCharacters = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd).Text.Length;

            StatusTextBlock.Text = $"Document length: {amountCharacters} characters";
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;

            var textToBold = new TextRange(ContentRichTextBox.Selection.Start, ContentRichTextBox.Selection.End);
            if (isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            else
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);

        }

        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedState = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            boldButton.IsChecked = (selectedState != DependencyProperty.UnsetValue) && (selectedState.Equals(FontWeights.Bold));

            var selectedStyle = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            italicButton.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            var selectedDecoration = ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineButton.IsChecked = (selectedDecoration != DependencyProperty.UnsetValue) && (selectedDecoration.Equals(TextDecorations.Underline));

            fontFamiltyComboBox.SelectedItem = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            fontSizeComboBox.Text = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty).ToString();
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            else
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            else
            {
                TextDecorationCollection textDecorations;
                (ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void FontFamiltyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontFamiltyComboBox.SelectedItem != null)
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamiltyComboBox.SelectedItem);
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);//text such that user can change it to their own value. 
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, $"{viewModel.SelectedNote.Id}.rtf");
            viewModel.SelectedNote.Filelocation = rtfFile;

            FileStream filestream = new FileStream(rtfFile, FileMode.Create);
            TextRange range = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
            range.Save(filestream, DataFormats.Rtf);

            viewModel.UpdateSelectedNode();
        }
    }
}
