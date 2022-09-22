﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bitpanda2Parqet
{
    public class MainViewPresenter
    {

        // An object of this class manages the communication between the loaded data (DataModel) and the Form (MainView)
        

        private MainView _mainView;
        private DataModel _dataModel;
        private BackgroundWorker _worker;
        private ParqetApiResults _results;


        public MainViewPresenter(MainView mainView)
        {
            _mainView = mainView;
            _dataModel = new DataModel(new List<Activity>());
            _worker = new BackgroundWorker();
            _results = new ParqetApiResults(0,0,0,0,0);

            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;

            _mainView.ParquetExportRequested += new EventHandler<MainViewParameters>(OnParqetExportRequested);
            _mainView.ParquetSynchRequested += new EventHandler<MainViewParameters>(OnParqetSyncRequested);
            _mainView.LoadInitRequested += new EventHandler(OnLoadInitRequested);
            _mainView.SaveInitRequested += new EventHandler<MainViewParameters>(OnSaveInitRequested);
            _worker.DoWork += new DoWorkEventHandler(OnDoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnRunWorkerCompleted);

            OnLoadInitRequested(this, new EventArgs());
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null )
            {
                MainView.ShowErrorMessage("Sync not successful\n" + e.Error.InnerException.Message);
            }
            else
            {
                MainView.ShowTextMessage("Sync finished");
            }
            
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _mainView.SetProgress(e.ProgressPercentage);
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                MainViewParameters info = (MainViewParameters)e.Argument;
                DataExchanger.UploadDataToParqetAPI(_dataModel.activities, info.Sync.ParqetAcc, info.Sync.ParqetToken, _worker, _results).Wait();
                MainView.ShowTextMessage(_results.ToString()); 
            }
            catch (Exception ex)
            {
                throw ex;               
            }          
        }

        private void OnSaveInitRequested(object sender, MainViewParameters e)
        {
            FormInitializer.SaveInitValues(e);
        }

        private void OnLoadInitRequested(object sender, EventArgs e)
        {
            _mainView.SetInitValues(FormInitializer.ReadMainViewInitValues());
        }

        private void OnParqetSyncRequested(object sender, MainViewParameters e)
        {
            try
            {
                if (!_worker.IsBusy)
                {
                    _dataModel.SetNewDataList(DataExchanger.DownloadDataFromBitpandaAPI(e.Sync.API, out BitpandaApiResults result));
                    MainView.ShowTextMessage(result.ToString());
                    _worker.RunWorkerAsync(e);
                }
            }
            catch (Exception ex)
            {
                MainView.ShowErrorMessage("Fehler beim Laden der Daten!\n" + ex.Message);
            }
        }

        private void OnParqetExportRequested(object sender, MainViewParameters e)
        {
            try
            {
                _dataModel.SetNewDataList(DataExchanger.DownloadDataFromBitpandaAPI(e.Sync.API, out BitpandaApiResults result));
                MainView.ShowTextMessage(result.ToString());
                _dataModel.ExportParqetCSV(e.Sync.FilePath);
                _mainView.SetProgress(100);
                MainView.ShowTextMessage("Laden abgeschlossen!");
            }
            catch (Exception ex)
            {
                MainView.ShowErrorMessage("Fehler beim Laden der Daten!\n" + ex.Message);
            }
        }

    


    }
}
