using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using EVE.Net;
using EVE.Net.Character;

namespace Elinor
{
    /// <summary>
    /// Interaction logic for ApiImportWindow.xaml
    /// </summary>
    public partial class ApiImportWindow
    {
        public Settings Settings = new Settings();

        public ApiImportWindow()
        {
            InitializeComponent();
            Settings.Accounting = 0;
            Settings.BrokerRelations = 0;
        }

        private void BtnGetCharsClick(object sender, RoutedEventArgs e)
        {
            cbChars.Items.Clear();
            try
            {
                var keyid = tbKeyId.Text;
                var vcode = tbVCode.Text;
          
                var info = new APIKeyInfo(keyid.ToString(CultureInfo.InvariantCulture), vcode);
                info.Query();
                
                if(info.characters.Count == 0)
                {
                    MessageBox.Show("No characters for this API information.\nPlease check you API information", "No characters found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    lblChar.Visibility = Visibility.Visible;
                    cbChars.Visibility = Visibility.Visible;

                    foreach(APIKeyInfo.Character chr in info.characters)
                    {
                        var chara = new CharWrapper
                                        {
                                            KeyId = keyid,
                                            VCode = vcode,
                                            Charname = chr.characterName,
                                            CharId = chr.characterID
                                        };

                        cbChars.Items.Add(chara);
                        cbChars.SelectedIndex = 0;
                    }
                    if(cbChars.Items.Count > 0)
                    {
                        btnOk.IsEnabled = true;
                        Topmost = false;
                    }
                }
            }
            catch(FormatException)
            {
                MessageBox.Show("Key ID must be a number", "Invalid Key ID", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
            
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            tbKeyId.Focus();
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            bool? success = true;
            btnOk.IsEnabled = false;
            pbLoading.Visibility = Visibility.Visible;
            var chara = (CharWrapper)cbChars.SelectedItem;

            var worker = new BackgroundWorker();
            worker.RunWorkerCompleted += delegate
                                             {
                                                 if(success == true) DialogResult = true;
                                                 else Dispatcher.Invoke(new Action(Close));
                                             };
            worker.DoWork += delegate
            {
                var sheet = new CharacterSheet(chara.KeyId, chara.VCode, chara.CharId.ToString(CultureInfo.InvariantCulture));
                sheet.Query();

                foreach (CharacterSheet.Skill skill in sheet.skills)
                {
                    if (skill.typeID == 3446) //"Broker Relations"
                        Settings.BrokerRelations = skill.level;
                    if (skill.typeID == 16622) //"Accounting" 
                        Settings.Accounting = skill.level;
                }

                Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     var aisfw =
                                                         new ApiImportSelectFactionWindow(chara)
                                                             {
                                                                 Topmost = true,
                                                                 Top=Top+10,
                                                                 Left = Left+10,
                                                             };
                success = aisfw.ShowDialog();
                if (success == true)
                {
                    Settings.CorpStanding = aisfw.Corp;
                    Settings.FactionStanding = aisfw.Faction;
                }
                else
                {
                    success = false;
                }
                }));

                Settings.ProfileName = chara.Charname;
           };

            worker.RunWorkerAsync();
        }

        private void BtnCreateKeyClick(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://support.eveonline.com/api/Key/CreatePredefined/524296/0/false");
        }
    }
}
