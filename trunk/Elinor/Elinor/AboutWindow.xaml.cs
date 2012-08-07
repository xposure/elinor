using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Elinor
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        private string charname = "Virppi Jouhinen";

        public AboutWindow()
        {
            InitializeComponent();
            SetLinearGradientUnderline();

            Assembly assembly = Assembly.GetExecutingAssembly();
FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
string version = fvi.ProductVersion;

            lblVersion.Content = version;

        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Escape)
            {
                Close();    
            }
        }

        private void LblCharMouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(charname);
        }

        private void LblCharMouseEnter(object sender, MouseEventArgs e)
        {
            lblChar.Content = CreateUnderlinedTextBlock(charname);
        }

        private TextBlock CreateUnderlinedTextBlock(string text)
        {
            TextDecoration myUnderline = new TextDecoration();
            
            myUnderline.Pen = new Pen(Brushes.Blue, 1);
            myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            // Set the underline decoration to a TextDecorationCollection and add it to the text block.
            TextDecorationCollection myCollection = new TextDecorationCollection();
            myCollection.Add(myUnderline);

            TextBlock blockHead = new TextBlock();
            blockHead.TextDecorations = myCollection;
            blockHead.Text = text;
            return blockHead;
        }

        private void SetLinearGradientUnderline()
        {
            // Create an underline text decoration. Default is underline.
            TextDecoration myBaseLine = new TextDecoration();
            TextDecoration myUnderLine = new TextDecoration();

            // Create a linear gradient pen for the text decoration.
            Pen gy = new Pen();
            gy.Brush = new LinearGradientBrush(Colors.Green, Colors.Yellow, new Point(0, 0.5), new Point(1, 0.5));
            gy.Brush.Opacity = 0.5;
            gy.Thickness = .75;
            gy.DashStyle = DashStyles.DashDotDot;
            myBaseLine.Pen = gy;
            myBaseLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;
            myBaseLine.Location = TextDecorationLocation.OverLine;

            Pen yr = new Pen();
            yr.Brush = new LinearGradientBrush(Colors.Yellow, Colors.Red, new Point(0, 0.5), new Point(1, 0.5));
            yr.Brush.Opacity = 0.5;
            yr.Thickness = .75;
            yr.DashStyle = DashStyles.DashDotDot;
            myUnderLine.Pen = yr;
            myUnderLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;


            // Set the underline decoration to a TextDecorationCollection and add it to the text block.
            TextDecorationCollection myCollection = new TextDecorationCollection();
            myCollection.Add(myBaseLine);
            myCollection.Add(myUnderLine);
            TextBlock block = new TextBlock();
            block.TextDecorations = myCollection;
            block.Text = "Elinor";
            label1.Content = block;
        }

        private void LblCharMouseLeave(object sender, MouseEventArgs e)
        {
            lblChar.Content = charname;
        }
    }
}
