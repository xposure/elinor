using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Elinor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DirectoryInfo _logdir = new DirectoryInfo(Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\EVE\logs\marketlogs");
        private readonly DirectoryInfo _profdir = new DirectoryInfo("profiles");
        private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        internal Settings Settings { get; set; }
        private FileSystemEventArgs _lastEvent;

        double _sell, _buy;

        public MainWindow()
        {
            InitializeComponent();
            if (_logdir.Parent != null && !_logdir.Parent.Exists)
            {
                if (Properties.Settings.Default.logpath != "")
                {
                    string path = Properties.Settings.Default.logpath;
                    try
                    {
                        _logdir = new DirectoryInfo(path);
                        SetWatcherAndStuff();
                    }
                    catch(Exception)
                    {
                        MessageBox.Show("Settings file corrupted, sorry.");
                    }
                }
                else
                {
                    var dlg = new SelectLogPathWindow();
                    var showDialog = dlg.ShowDialog();
                    if(showDialog != null && (bool) showDialog)
                    {
                        _logdir = dlg.Logpath;
                        Properties.Settings.Default.logpath = _logdir.FullName;
                        Properties.Settings.Default.Save();
                        SetWatcherAndStuff();
                    }

                }
            }
            else
            {
                SetWatcherAndStuff();
            }
        }

        private void SetWatcherAndStuff()
        {
            if (!_logdir.Exists)
                _logdir.Create();
            if (!_profdir.Exists)
                _profdir.Create();

            _fileSystemWatcher.Path = _logdir.FullName;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.EnableRaisingEvents = true;

            dgSamples.ColumnWidth = DataGridLength.SizeToCells;
            dgSamplesFive.ColumnWidth = DataGridLength.SizeToCells;
            UpdateStatus();
        }

        private void ProcessData(string s)
        {
            List<List<string>> table = CSVReader.GetTableFromCSV(s);

            if (table == null) return;

            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 var sell = from List<string> row in table
                                                            where row[7] == "False" && row[13] == "0"
                                                            orderby
                                                                double.Parse(row[0], CultureInfo.InvariantCulture) ascending
                                                            select row;
                                                 string sss = sell.Any() ? sell.ElementAt(0)[0] : "1.0";
                                                 _sell = double.Parse(sss, CultureInfo.InvariantCulture);
                                                 lblSell.Content = String.Format("{0:n} ISK", _sell);

                                                 var buy = from List<string> row in table
                                                           where row[7] == "True" && row[13] == "0"
                                                           orderby
                                                               double.Parse(row[0], CultureInfo.InvariantCulture) descending
                                                           select row;
                                                 string bbb = buy.Any() ? buy.ElementAt(0)[0] : "1.0";

                                                 _buy = double.Parse(bbb, CultureInfo.InvariantCulture);
                                                 lblBuy.Content = String.Format("{0:n} ISK", _buy);
                                             }));

            var cdt = new CalculateDataThread(_sell, _buy, this);
            var calc = new Thread(cdt.Run);
            calc.Start();
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 if (cbAutoCopy.IsChecked != null && (bool)cbAutoCopy.IsChecked)
                                                 {
                                                     var img = new BitmapImage();
                                                     img.BeginInit();
                                                     img.UriSource =
                                                         new Uri(
                                                             "pack://application:,,,/Elinor;component/Images/38_16_195.png");
                                                     img.EndInit();
                                                     imgCopyStatus.Source = img;
                                                 }
                                             }));

            _lastEvent = fileSystemEventArgs;
            while (IsFileLocked(new FileInfo(fileSystemEventArgs.FullPath))) Thread.Sleep(25);
            if (fileSystemEventArgs.ChangeType == WatcherChangeTypes.Created &&
                fileSystemEventArgs.Name.EndsWith(".txt"))
            {
                ProcessData(fileSystemEventArgs.FullPath);
            }

            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 if (cbAutoCopy.IsChecked != null && (bool)cbAutoCopy.IsChecked)
                                                 {

                                                     if (rbSell.IsChecked != null && (bool)rbSell.IsChecked)
                                                         SetClipboardWrapper(ClipboardTools.GetSellPrice(_sell, Settings));
                                                     else if (rbBuy.IsChecked != null && (bool)rbBuy.IsChecked)
                                                         SetClipboardWrapper(ClipboardTools.GetBuyPrice(_buy, Settings));


                                                     var img = new BitmapImage();
                                                     img.BeginInit();
                                                     img.UriSource = new Uri("pack://application:,,,/Elinor;component/Images/38_16_193.png");
                                                     img.EndInit();
                                                     imgCopyStatus.Source = img;
                                                 }
                                             }));
            UpdateStatus();
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }


        private void UpdateStatus()
        {
            long size = _logdir.GetFiles().Sum(fi => fi.Length);

            Dispatcher.Invoke(new Action(delegate { tbStatus.Text = String.Format("Market logs: {0:n0} KB", size / 1024); }));
        }
        
        private void TbStatusMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (FileInfo fi in _logdir.GetFiles())
            {
                if (!IsFileLocked(fi))
                    fi.Delete();
            }
            UpdateStatus();
        }

        private void BtnStayOnTopClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 if (btnStayOnTop.IsChecked != null)
                                                     Topmost = (bool)btnStayOnTop.IsChecked;
                                             }));
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Settings.SaveSettings(Settings);
        }

        private void TiSettingsLostFocus(object sender, RoutedEventArgs e)
        {
            Settings.SaveSettings(Settings);
        }

        private void TiSettingsGotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 slMargin.Value = Settings.MarginThreshold;
                                                 slMinimum.Value = Settings.MinimumThreshold;

                                                 slCorpStanding.Value = Settings.CorpStanding;
                                                 slFactionStanding.Value = Settings.FactionStanding;

                                                 cbBrokerRelations.SelectedIndex = Settings.BrokerRelations;
                                                 cbAccounting.SelectedIndex = Settings.Accounting;

                                                 cbAdvancedSettings.IsChecked = Settings.AdvancedStepSettings;
                                                 tbSellFract.Text = (Settings.SellPercentage * 100).ToString(CultureInfo.InvariantCulture);
                                                 tbBuyFract.Text = (Settings.BuyPercentage * 100).ToString(CultureInfo.InvariantCulture);
                                                 tbSellThresh.Text = Settings.SellThreshold.ToString(CultureInfo.InvariantCulture);
                                                 tbBuyThresh.Text = Settings.BuyThreshold.ToString(CultureInfo.InvariantCulture);
                                             }));
        }

        private void SlMarginValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.MarginThreshold = slMargin.Value;
            Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     slMinimum.Maximum = slMargin.Value;
                                                     lblMarginSetting.Content = String.Format("{0:n}%", slMargin.Value * 100);
                                                 }));
        }

        private void SlMinimumValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.MinimumThreshold = slMinimum.Value;
            Dispatcher.Invoke(new Action(delegate
            {
                lblMinimumSetting.Content = String.Format("{0:n}%", slMinimum.Value * 100);
            }));
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            slMargin.ValueChanged += SlMarginValueChanged;
            slMinimum.ValueChanged += SlMinimumValueChanged;

            slCorpStanding.ValueChanged += SlCorpStandingValueChanged;
            slFactionStanding.ValueChanged += SlFactionStandingValueChanged;

            cbBrokerRelations.SelectionChanged += CbBrokerRelationsSelectionChanged;
            cbAccounting.SelectionChanged += CbAccountingSelectionChanged;

            for (int i = 0; i < 6; i++)
            {
                cbBrokerRelations.Items.Add(i);
                cbAccounting.Items.Add(i);
            }

            Settings = new Settings();
            cbProfiles.Items.Add(Settings);
            cbProfiles.SelectedIndex = 0;

            ShowTutorialHint();

            UpdateProfiles();
        }

        private void ShowTutorialHint()
        {
            if (Properties.Settings.Default.showtutorial)
            {
                Properties.Settings.Default.showtutorial = false;
                Properties.Settings.Default.Save();
                Tutorial.FlashControl(btnTutorial, Colors.Yellow, this);
                var tutHint = new Popup
                                    {
                                        VerticalOffset = -3,
                                        PlacementTarget = btnTutorial,
                                        Placement = PlacementMode.Top,
                                        IsOpen = true
                                    };
                var brd = new Border
                                      {

                                          BorderBrush =
                                              new LinearGradientBrush(Colors.LightSlateGray, Colors.Black, .45),
                                          BorderThickness = new Thickness(1),
                                          Background =
                                              new LinearGradientBrush(Colors.LightYellow, Colors.PaleGoldenrod, .25),
                                          Child = new TextBlock
                                                      {
                                                          Margin = new Thickness(4),
                                                          FontSize = 12,
                                                          Text = "Click to start a short tutorial on how to use Elinor"
                                                      }
                                      };
                tutHint.Child = brd;
                tutHint.MouseDown += delegate
                                         {
                                             tutHint.IsOpen = false;
                                         };
            }
        }

        private void UpdateProfiles()
        {
            foreach (FileInfo file in _profdir.GetFiles())
            {
                Settings settings = Settings.ReadSettings(file.Name.Replace(file.Extension, ""));
                if (settings != null)
                {
                    cbProfiles.Items.Add(settings);
                }
            }
        }

        private void CbBrokerRelationsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.BrokerRelations = cbBrokerRelations.SelectedIndex;
            UpdateBrokerFee();
        }

        private void BtnDefaultClick(object sender, RoutedEventArgs e)
        {
            var tSettings = new Settings { ProfileName = Settings.ProfileName };
            Settings = tSettings;
            Settings.SaveSettings(Settings);
            TiSettingsGotFocus(this, null);
        }

        private void UpdateBrokerFee()
        {
            Dispatcher.Invoke(new Action(delegate
            {
                lblBrokerRelations.Content = String.Format("Broker fee: {0:n}%", CalculateDataThread.BrokerFee(Settings.BrokerRelations,
                    Settings.CorpStanding, Settings.FactionStanding) * 100);
            }));


        }

        private void SlCorpStandingValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.CorpStanding = slCorpStanding.Value;
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 lblCorpStanding.Content = String.Format("{0:n}", slCorpStanding.Value);
                                             }));

            UpdateBrokerFee();
        }

        private void SlFactionStandingValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.FactionStanding = slFactionStanding.Value;
            Dispatcher.Invoke(new Action(delegate
            {
                lblFactionStanding.Content = String.Format("{0:n}", slFactionStanding.Value);
            }));

            UpdateBrokerFee();
        }

        private void CbAccountingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Accounting = cbAccounting.SelectedIndex;
            Dispatcher.Invoke(new Action(delegate
            {
                lblSalesTax.Content = String.Format("Sales tax: {0:n}%", CalculateDataThread.SalesTax(Settings.Accounting) * 100);
            }));
        }

        private void CbProfilesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.SaveSettings(Settings);
            Settings = (Settings)cbProfiles.SelectedItem;
            TiSettingsGotFocus(this, null);
            if (_lastEvent != null) FileSystemWatcherOnCreated(this, _lastEvent);
        }

        private void BtnNewClick(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            var pnw = new ProfileNameWindow
                          {
                              Top = Top + Height / 10,
                              Left = Left + Width / 10,
                              Topmost = Topmost,
                          };
            if (pnw.ShowDialog() == true)
            {
                settings.ProfileName = pnw.ProfileName;
                cbProfiles.Items.Add(settings);
                cbProfiles.SelectedItem = settings;
                tcMain.SelectedIndex = 1;
                Settings.SaveSettings(settings);
            }
        }

        private void BtnImportClick(object sender, RoutedEventArgs e)
        {
            var aiw = new ApiImportWindow
                          {
                              Topmost = Topmost,
                              Top = Top + Height / 10,
                              Left = Left + Width / 10,
                          };

            if (aiw.ShowDialog() == true)
            {
                Settings settings = aiw.Settings;
                string fName = string.Format("profiles\\{0}.dat", settings.ProfileName);
                if (File.Exists(fName))
                {
                    MessageBoxResult result = MessageBox.Show("Character exists. Update?",
                                    "Character already exists", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        int index = 1;
                        File.Delete(fName);
                        for (int i = 0; i < cbProfiles.Items.Count; i++)
                        {
                            var tmp = (Settings)cbProfiles.Items[i];
                            if (tmp.ProfileName == settings.ProfileName)
                            {
                                index = i;
                                cbProfiles.SelectedIndex = 0;
                                cbProfiles.Items.RemoveAt(i);
                                break;
                            }
                        }
                        cbProfiles.Items.Insert(index, settings);
                        cbProfiles.SelectedItem = settings;
                        tcMain.SelectedIndex = 1;
                    }
                }
                else
                {
                    cbProfiles.Items.Add(settings);
                    cbProfiles.SelectedItem = settings;
                    tcMain.SelectedIndex = 1;
                }
            }
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e)
        {
            Settings tSet = Settings;
            int i = cbProfiles.SelectedIndex;
            cbProfiles.SelectedIndex = i - 1;
            cbProfiles.Items.RemoveAt(i);
            File.Delete("profiles\\" + tSet.ProfileName + ".dat");
        }

        private void BtnAboutClick(object sender, RoutedEventArgs e)
        {
            var abt = new AboutWindow
            {
                Topmost = Topmost,
                Top = Top + Height / 10,
                Left = Left + Width / 10
            };
            abt.ShowDialog();
        }

        private void PinWindow(object sender, ExecutedRoutedEventArgs e)
        {
            btnStayOnTop.IsChecked = !btnStayOnTop.IsChecked;
            BtnStayOnTopClick(this, null);
        }

        private void CbAutoCopyChecked(object sender, RoutedEventArgs e)
        {
            gbAutocopy.IsEnabled = true;
            imgCopyStatus.Source = null;
        }

        private void CbAutoCopyUnchecked(object sender, RoutedEventArgs e)
        {
            gbAutocopy.IsEnabled = false;
            imgCopyStatus.Source = null;
        }

        private void AutoCopy(object sender, ExecutedRoutedEventArgs e)
        {
            cbAutoCopy.IsChecked = !cbAutoCopy.IsChecked;
        }

        private void LblSellMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetClipboardWrapper(ClipboardTools.GetSellPrice(_sell, Settings));
        }

        private void LblBuyMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetClipboardWrapper(ClipboardTools.GetBuyPrice(_buy, Settings));
        }

        private void MiSubmitBugClick(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://code.google.com/p/elinor/issues/list");
        }

        private void MiSubmitFeatureClick(object sender, RoutedEventArgs e)
        {
            Process.Start(@"http://redd.it/xl6mf");
        }

        private void RbChecked(object sender, RoutedEventArgs e)
        {
            double price = rbSell.IsChecked != null && (bool)rbSell.IsChecked ? ClipboardTools.GetSellPrice(_sell, Settings) : ClipboardTools.GetBuyPrice(_buy, Settings);
            SetClipboardWrapper(price);
        }

        private void SetClipboardWrapper(double d)
        {
            if (d > .01)
            {
                Clipboard.SetText(d.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void BtnTutorialClick(object sender, RoutedEventArgs e)
        {
            Tutorial.Main = this;
            Tutorial.NextTip();
        }

        private void CbAdvancedSettingsChecked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This is a barely tested feature.\nPlease use with caution and report any bugs.",
                            "Experimental feature", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                Settings.AdvancedStepSettings = true;
                gbAdvancedSettings.IsEnabled = true;
            }
            else
            {
                cbAdvancedSettings.IsChecked = false;
            }
        }

        private void CbAdvancedSettingsUnchecked(object sender, RoutedEventArgs e)
        {
            Settings.AdvancedStepSettings = false;
            gbAdvancedSettings.IsEnabled = false;
        }
        
        private void TbSellFractTextChanged(object sender, TextChangedEventArgs e)
        {
            double fract;
            if (double.TryParse(tbSellFract.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out fract))
            {
                Settings.SellPercentage = fract/100;
            }
        }

        private void TbSellThreshTextChanged(object sender, TextChangedEventArgs e)
        {
            double thresh;
            if (double.TryParse(tbSellThresh.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out thresh))
            {
                Settings.SellThreshold = thresh;
            }
        }

        private void TbBuyFractTextChanged(object sender, TextChangedEventArgs e)
        {
            double fract;
            if (double.TryParse(tbBuyFract.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out fract))
            {
                Settings.BuyPercentage = fract / 100;
            }
        }

        private void TbBuyThreshTextChanged(object sender, TextChangedEventArgs e)
        {
            double thresh;
            if (double.TryParse(tbBuyThresh.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out thresh))
            {
                Settings.BuyThreshold = thresh;
            }
        }
    }
    
}
