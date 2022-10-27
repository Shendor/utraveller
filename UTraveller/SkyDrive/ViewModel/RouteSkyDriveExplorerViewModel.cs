using Microsoft.Live;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Common.Message;
using UTraveller.Common.Util;
using UTraveller.Service.Api;
using UTraveller.Service.Model;
using UTraveller.SkyDrive.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.SkyDrive.ViewModel
{
    public class RouteSkyDriveExplorerViewModel : SkyDriveExplorerViewModel
    {
        private IRouteFileReader routeFileReader;
        private ITaskExecutionManager taskExecutionManager;
        private ICancelableTaskProgressService progressService;
        private IZipExtractService zipExtractService;

        public RouteSkyDriveExplorerViewModel(IFileExplorerService<OneDriveFileRequest> skyDriveExplorerService,
                 IRouteFileReader routeFileReader, ITaskExecutionManager taskExecutionManager,
                 ICancelableTaskProgressService progressService,
                 ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken,
                 [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService,
                 IZipExtractService zipExtractService)
            : base(skyDriveExplorerService, liveSocialAccessToken, backgroundTaskProgressService)
        {
            this.routeFileReader = routeFileReader;
            this.taskExecutionManager = taskExecutionManager;
            this.progressService = progressService;
            this.zipExtractService = zipExtractService;
        }


        public override async void Initialize()
        {
            if (liveSocialAccessToken.AccessToken != null)
            {
                await Initialize(new RouteSkyDriveFileRequest(liveSocialAccessToken.AccessToken));
            }
        }


        internal override async void OpenFile(OneDriveFileRequest request, string fileName)
        {
            progressService.RunIndeterminateProgress("Reading route...");
            try
            {
                var stream = await skyDriveExplorerService.ReadFile(request);
                if (!progressService.IsCanceled)
                {
                    var newStream = request.ChosenExtension.Equals(FileExtensionType.KMZ.ToString().ToLower()) ?
                        await zipExtractService.Unzip(stream) : stream;
                    var routeInfo = await routeFileReader.ReadRoute(newStream);
                    if (!progressService.IsCanceled)
                    {
                        MessengerInstance.Send<RouteAddedMessage>(new RouteAddedMessage(routeInfo));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                progressService.FinishProgress();
            }
        }


        protected override OneDriveFileRequest CreateRequestOnSkyDriveSelectedItemChanged(SkyDriveItemChangedMessage message)
        {
            return new RouteSkyDriveFileRequest(message.Object.Id, liveSocialAccessToken.AccessToken, message.Object.Extension);
        }
    }
}
