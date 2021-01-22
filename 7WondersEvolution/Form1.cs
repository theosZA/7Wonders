using System;
using System.Linq;
using System.Windows.Forms;

namespace _7WondersEvolution
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCreatePlayers_Click(object sender, EventArgs e)
        {
            CreatePlayers();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayGames();
        }

        private void btnNextGeneration_Click(object sender, EventArgs e)
        {
            if (players == null)
            {
                CreatePlayers();
                Refresh();
            }
            if (!players.AnyGamesPlayed)
            {
                PlayGames();
                Refresh();
            }

            for (int i = 0; i < (int)nudGenerations.Value; ++i)
            {
                players.ReplaceWithNewGeneration();
                OnNewGeneration();
                PlayGames();
                Refresh();
            }
        }

        private void CreatePlayers()
        {
            players = new PlayerPool((int)nudPlayers.Value);
            OnNewGeneration();
        }

        private void PlayGames()
        {
            int gamesToPlay = (int)nudGamesPlayed.Value;
            players.PlayGamesWithRandomPlayers(gamesToPlay, playerCount: 7);
            OnGamesPlayed();
        }

        private void OnNewGeneration()
        {
            dgvPlayers.Rows.Clear();
            foreach (var info in players.Info)
            {
                dgvPlayers.Rows.Add(info.generation, info.name);
            }
            ++generation;
            txtGeneration.Text = $"{generation}";
        }

        private void OnGamesPlayed()
        {
            var stats = players.Stats.ToList();
            for (int rowIndex = 0; rowIndex < stats.Count; ++rowIndex)
            {
                var row = dgvPlayers.Rows[rowIndex];
                row.Cells[2].Value = stats[rowIndex].games;
                row.Cells[3].Value = $"{stats[rowIndex].averagePosition:0.000}";
                row.Cells[4].Value = $"{stats[rowIndex].averageVictoryPoints:0.000}";
            }
            txtAverageVPs.Text = $"{stats.Sum(stat => stat.averageVictoryPoints * stat.games) / stats.Sum(stat => stat.games)}";
        }

        private PlayerPool players;

        private int generation = 0;
    }
}
