using System;
using System.ComponentModel;
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

        private void btnNextGeneration_Click(object sender, EventArgs e)
        {
            var evolution = new Evolution((int)nudPlayers.Value, (int)nudGamesPlayed.Value);
            backgroundWorker.RunWorkerAsync((evolution, (int)nudGenerations.Value));
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            (var evolution, int generations) = ((Evolution, int))e.Argument;
            for (int i = 0; i < generations; ++i)
            {
                evolution.AdvanceGeneration();
                backgroundWorker.ReportProgress(100 * evolution.Generation / generations, (evolution.GetCopyOfPlayers(), evolution.Generation));
            }
            e.Result = (evolution.GetCopyOfPlayers(), evolution.Generation);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            (var players, int generation) = ((PlayerPool, int))e.UserState;
            RenderPlayerPool(players, generation);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (var players, int generation) = ((PlayerPool, int))e.Result;
            RenderPlayerPool(players, generation);
        }

        private void RenderPlayerPool(PlayerPool players, int generation)
        {
            txtGeneration.Text = $"{generation}";

            dgvPlayers.Rows.Clear();
            foreach (var info in players.Info)
            {
                dgvPlayers.Rows.Add(info.generation, info.cityName, info.name);
            }
            var stats = players.Stats.ToList();
            for (int rowIndex = 0; rowIndex < stats.Count; ++rowIndex)
            {
                var row = dgvPlayers.Rows[rowIndex];
                row.Cells[3].Value = stats[rowIndex].games;
                row.Cells[4].Value = $"{stats[rowIndex].averagePosition:0.000}";
                row.Cells[5].Value = $"{stats[rowIndex].averageVictoryPoints:0.000}";
            }
            txtAverageVPs.Text = $"{stats.Sum(stat => stat.averageVictoryPoints * stat.games) / stats.Sum(stat => stat.games)}";

            dgvPlayers.Sort(dgvPlayers.Columns[5], ListSortDirection.Descending);
        }
    }
}
