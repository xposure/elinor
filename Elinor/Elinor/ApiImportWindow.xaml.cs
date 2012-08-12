using System;
using System.ComponentModel;
using System.Windows;
using EveAI.Live;
using EveAI.Live.Account;
using EveAI.Live.Character;

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
                long keyid = long.Parse(tbKeyId.Text);
                string vcode = tbVCode.Text;

                EveApi api = new EveApi(keyid, vcode);
                APIKeyInfo info = api.getApiKeyInfo();
                if(info.Characters.Count == 0)
                {
                    MessageBox.Show("No characters for this API information.\nPlease check you API information", "No characters found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    lblChar.Visibility = Visibility.Visible;
                    cbChars.Visibility = Visibility.Visible;

                    foreach(AccountEntry ae in info.Characters)
                    {
                        CharWrapper chara = new CharWrapper();
                        chara.KeyId = keyid;
                        chara.VCode = vcode;
                        chara.Charname = ae.Name;
                        chara.CharId = ae.CharacterID;

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
            btnOk.IsEnabled = false;
            pbLoading.Visibility = Visibility.Visible;
            CharWrapper chara = (CharWrapper)cbChars.SelectedItem;

            BackgroundWorker worker = new BackgroundWorker();

            worker.RunWorkerCompleted += delegate { DialogResult = true; };

            worker.DoWork += delegate
            {
                EveApi api = new EveApi(chara.KeyId, chara.VCode, chara.CharId);
                
                foreach (CharacterSheet.LearnedSkill skill in api.GetCharacterSheet().Skills)
                {
                    if (skill.Skill.Name == "Broker Relations") 
                        Settings.BrokerRelations = skill.Level;
                    if (skill.Skill.Name == "Accounting") 
                        Settings.Accounting = skill.Level;
                }

                Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     ApiImportSelectFactionWindow aisfw =
                                                         new ApiImportSelectFactionWindow(chara)
                                                             {
                                                                 Topmost = true,
                                                                 Top=Top+10,
                                                                 Left = Left+10,
                                                             };
                if (aisfw.ShowDialog() == true)
                {
                    Settings.CorpStanding = aisfw.Corp;
                    Settings.FactionStanding = aisfw.Faction;
                }
                }));
                Settings.ProfileName = chara.Charname;
           };

            worker.RunWorkerAsync();
        }
    }
}
