﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bitpanda2Parqet
{
    public partial class MainView : Form
    {
        public event EventHandler<MainViewParameters> CSVExportRequested;
        public event EventHandler<MainViewParameters> ParquetSyncRequested;
        public event EventHandler LoadInitRequested;
        public event EventHandler<MainViewParameters> SaveInitRequested;

        public MainView()
        {
            InitializeComponent();
        }


        private void btnCSVExport_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txbFileName.Text))
            {
                MessageBox.Show("Zuerst Dateiname eingeben!");
            }
            else if (String.IsNullOrWhiteSpace(txbBitpandaAPI.Text))
            {
                MessageBox.Show("Zuerst API eingeben!");
            }
            else if (String.IsNullOrWhiteSpace(txbFilePath.Text))
            {
                MessageBox.Show("Zuerst Pfad auswählen!");
            }
            else
            {
                CSVExportRequested?.Invoke(this, GetMainViewParameters());
            }

        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txbFilePath.Text = fbd.SelectedPath; ;
                }
            }
        }

        public static void ShowTextMessage(string message)
        {
            MessageBox.Show(message, "Mitteilung", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnParqetSync_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txbBitpandaAPI.Text))
            {
                MessageBox.Show("Zuerst API eingeben!");
            }
            else if (String.IsNullOrWhiteSpace(txbParqetAcc.Text))
            {
                MessageBox.Show("Zuerst Parqet Accountnummer eingeben!");
            }
            else if (String.IsNullOrWhiteSpace(txbParqetToken.Text))
            {
                MessageBox.Show("Zuerst Parqet Token eingeben!");
            }
            else
            {
                ParquetSyncRequested?.Invoke(this, GetMainViewParameters());
            }
        }

        public void SetInitValues(MainViewParameters init)
        {
            txbFileName.Text = init.Sync.FileName;
            txbFilePath.Text = init.Sync.FilePath;
            txbBitpandaAPI.Text = init.Sync.API;
            txbParqetAcc.Text = init.Sync.ParqetAcc;
            txbParqetToken.Text = init.Sync.ParqetToken;
            cbxExportFormat.SelectedItem = init.Settings.ExportFormat;
            dtpDataFromDate.Value = init.Settings.DateOfOldestData;
            clbGenerellSettings.SetItemChecked(0, init.Settings.IgnoreStaking);
        }

        private MainViewParameters GetMainViewParameters()
        { 
            MainViewSyncParameters parameters = new MainViewSyncParameters(txbBitpandaAPI.Text, txbFilePath.Text, txbFileName.Text, txbParqetAcc.Text, txbParqetToken.Text);
            MainViewSettingsParameters settings = new MainViewSettingsParameters((Enums.ExportFormat)cbxExportFormat.SelectedItem, dtpDataFromDate.Value, clbGenerellSettings.GetItemCheckState(0) == CheckState.Checked);

            return new MainViewParameters(parameters, settings);
        }

        private void btnSaveInitSettings_Click(object sender, EventArgs e)
        {
            SaveInitRequested?.Invoke(sender, GetMainViewParameters());
        }

        private void btnLoadInitSettings_Click(object sender, EventArgs e)
        {
            LoadInitRequested?.Invoke(sender, new EventArgs());
        }

        public void SetProgress(int progress)
        {
            pgbProgress.Value = progress;
        }
    }

}
