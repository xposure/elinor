using System.Windows;
using EveAI.Live;

namespace Elinor
{
    /// <summary>
    /// Interaction logic for ApiImportSelectFactionWindow.xaml
    /// </summary>
    public partial class ApiImportSelectFactionWindow
    {
        public double Faction;
        public double Corp;
        private CharWrapper chara;

        internal ApiImportSelectFactionWindow(CharWrapper chr)
        {
            InitializeComponent();
            chara = chr;

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            EveApi api = new EveApi(chara.KeyId, chara.VCode, chara.CharId);
            StandingWrapper none = new StandingWrapper("<None>", .0);
            cbCorp.Items.Add(none);
            cbFaction.Items.Add(none);
            cbCorp.SelectedIndex = 0;
            cbFaction.SelectedIndex = 0;

            foreach (Standing standing in api.GetCharacterStandings())
            {
                StandingWrapper wrap = new StandingWrapper(standing.EntityName, standing.Value);

                if (standing.Type == StandingType.NPCCorporations) cbCorp.Items.Add(wrap);
                if (standing.Type == StandingType.Factions) cbFaction.Items.Add(wrap);
            }
        }

        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            Faction = ((StandingWrapper) cbFaction.SelectedItem).Standing;
            Corp = ((StandingWrapper) cbCorp.SelectedItem).Standing;
            DialogResult = true;
            Close();
        }
    }
}
