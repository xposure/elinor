using System.Globalization;
using System.Windows;
using EVE.Net.Character;

namespace Elinor
{
    /// <summary>
    /// Interaction logic for ApiImportSelectFactionWindow.xaml
    /// </summary>
    public partial class ApiImportSelectFactionWindow
    {
        public double Faction;
        public double Corp;
        private readonly CharWrapper _chara;

        internal ApiImportSelectFactionWindow(CharWrapper chr)
        {
            InitializeComponent();
            _chara = chr;

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var none = new StandingWrapper("<None>", .0);
            cbCorp.Items.Add(none);
            cbFaction.Items.Add(none);
            cbCorp.SelectedIndex = 0;
            cbFaction.SelectedIndex = 0;

            var standings = new NPCStandings(_chara.KeyId, _chara.VCode, _chara.CharId.ToString(CultureInfo.InvariantCulture));
            standings.Query();

            foreach (var standing in standings.standings.NPCCorporations)
            {
                var wrap = new StandingWrapper(standing.fromName, standing.standing);
                cbCorp.Items.Add(wrap);
            }

            foreach (var standing in standings.standings.factions)
            {
                var wrap = new StandingWrapper(standing.fromName, standing.standing);
                cbFaction.Items.Add(wrap);
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
