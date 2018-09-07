using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.SharpDevelop.Editor;
using Noesis.Javascript;
using QuickJS.JavaScript;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

namespace QuickJS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JsExecutor jsExecutor = new JsExecutor();

        public MainWindow()
        {
            InitializeComponent();
            jsExecutor.Initialize((s) =>
            {
                ConsoleTextBox.AppendText(s + "\n");
            });
            textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(textEditor.Options);
            InitializeTextMarkerService();
        }

        ITextMarkerService textMarkerService;

        void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        private int FindIndex(int row)
        {
            var line = 0;
            var index = 0;
            foreach (var str in textEditor.Text.Split('\n'))
            {
                if (row == line)
                    return index;
                line++;
                index += str.Length;
            }
            throw new IndexOutOfRangeException();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            textMarkerService.RemoveAll(m => true);
            try
            {
                ConsoleTextBox.Foreground = Brushes.Black;
                ConsoleTextBox.Clear();
                jsExecutor.Execute(textEditor.Text);
            }
            catch (JavascriptException ex)
            {
                ConsoleTextBox.Foreground = Brushes.Red;
                ConsoleTextBox.AppendText($"line {ex.Line} col {ex.StartColumn}: {ex.Message}");
                var lineIndex = FindIndex(ex.Line - 1);
                ITextMarker marker = textMarkerService.Create(lineIndex + ex.StartColumn, ex.EndColumn - ex.StartColumn);
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
            }
        }
    }
}
