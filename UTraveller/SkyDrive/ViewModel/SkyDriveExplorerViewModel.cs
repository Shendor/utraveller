using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Live;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;
using UTraveller.Service.Model;
using UTraveller.SkyDrive.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.SkyDrive.ViewModel
{
    public abstract class SkyDriveExplorerViewModel : BaseViewModel
    {
        protected IDictionary<string, string> parentDictionary;
        protected ICollection<ISkyDriveItem> skyDriveItems;
        protected IFileExplorerService<OneDriveFileRequest> skyDriveExplorerService;
        protected ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken;
        private ITaskProgressService backgroundTaskProgressService;
        private bool isInitialized;

        public SkyDriveExplorerViewModel(IFileExplorerService<OneDriveFileRequest> skyDriveExplorerService,
            ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken,
            [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService)
        {
            this.skyDriveExplorerService = skyDriveExplorerService;
            this.liveSocialAccessToken = liveSocialAccessToken;
            this.backgroundTaskProgressService = backgroundTaskProgressService;

            skyDriveItems = new ObservableCollection<ISkyDriveItem>();
            parentDictionary = new Dictionary<string, string>();

            MessengerInstance.Register<SkyDriveItemChangedMessage>(this, OnSkyDriveItemChanged);
        }

        public ICollection<ISkyDriveItem> SkyDriveItems
        {
            get { return skyDriveItems; }
        }

        public override void Cleanup()
        {
            parentDictionary.Clear();
        }

        internal async Task Initialize(OneDriveFileRequest skyDriveRequest)
        {
            SkyDriveItems.Clear();
            if (liveSocialAccessToken.AccessToken != null)
            {
                try
                {
                    backgroundTaskProgressService.RunIndeterminateProgress();
                    var skyDriveFiles = await skyDriveExplorerService.GetFiles(skyDriveRequest);
                    if (parentDictionary.ContainsKey(skyDriveRequest.FileName))
                    {
                        var parentFolder = new File(parentDictionary[skyDriveRequest.FileName], "...", true);
                        SkyDriveItems.Add(new SkyDriveFolderViewModel(parentFolder));
                    }
                    foreach (var file in skyDriveFiles)
                    {
                        SkyDriveItems.Add(SkyDriveItemFactory.GetSkyDriveItem(file));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    backgroundTaskProgressService.FinishProgress();
                }
            }
        }

        public abstract void Initialize();

        internal abstract void OpenFile(OneDriveFileRequest request, string fileName);

        private async void OnSkyDriveItemChanged(SkyDriveItemChangedMessage message)
        {
            var request = CreateRequestOnSkyDriveSelectedItemChanged(message);
            if (message.Object.IsDirectory)
            {
                if (message.Object.ParentName != null)
                {
                    parentDictionary[message.Object.Id] = message.Object.ParentName;
                }
                await Initialize(request);
            }
            else
            {
                OpenFile(request, message.Object.Name);
            }
        }

        protected abstract OneDriveFileRequest CreateRequestOnSkyDriveSelectedItemChanged(SkyDriveItemChangedMessage message);

    }
}
