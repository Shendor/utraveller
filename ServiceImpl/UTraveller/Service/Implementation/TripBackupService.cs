using Microsoft.Live;
using Newtonsoft.Json;
using ServiceApi.UTraveller.Service.Exceptions;
using ServiceImpl.UTraveller.Service.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Exceptions;
using UTraveller.Service.Model;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class TripBackupService : ITripBackupService
    {
        private static readonly string DEFAULT_ALBUM_NAME = "uTraveler";
        private static readonly string FILE_NAME_PATTERN = @"(trip-){1}.+(\.utt){1}";

        private string folderUploadUrl;
        private ISocialClientAccessToken<LiveConnectSession> liveAccessToken;
        private IFileExplorerService<OneDriveFileRequest> oneDriveFileExplorerService;
        private IUserService userService;
        private IEventService eventService;
        private IPhotoService photoService;
        private IMessageService messageService;
        private IExpenseService moneySpendingService;
        private IRouteService routeService;
        private ITripPlanService tripPlanService;
        private ICancelableTaskProgressService taskProgressService;

        public TripBackupService(ISocialClientAccessToken<LiveConnectSession> liveAccessToken,
                                 IUserService userService, IEventService eventService,
                                IPhotoService photoService, IMessageService messageService,
                                IExpenseService moneySpendingService, IRouteService routeService,
                                ITripPlanService tripPlanService,
                                IFileExplorerService<OneDriveFileRequest> oneDriveFileExplorerService,
                                ICancelableTaskProgressService taskProgressService)
        {
            this.liveAccessToken = liveAccessToken;
            this.eventService = eventService;
            this.userService = userService;
            this.photoService = photoService;
            this.messageService = messageService;
            this.moneySpendingService = moneySpendingService;
            this.routeService = routeService;
            this.tripPlanService = tripPlanService;
            this.oneDriveFileExplorerService = oneDriveFileExplorerService;
            this.taskProgressService = taskProgressService;
        }


        public async Task<bool> Backup(Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser != null && liveAccessToken.AccessToken != null)
            {
                try
                {
                    taskProgressService.UpdateProgress(1, 100, "Start to back-up trip...");
                    var backupData = new BackupData();

                    backupData.Trip = e;
                    backupData.Photos = photoService.GetPhotos(e);
                    backupData.Messages = messageService.GetMessagesOfEvent(e);
                    backupData.Expenses = moneySpendingService.GetExpenses(e);
                    backupData.Routes = routeService.GetRoutes(e);
                    InitializeRouteData(backupData);
                    backupData.TripPlan = tripPlanService.GetTripPlan(e);

                    var backupDataJson = ConvertToJson(backupData);
                    return await WriteToFileInOneDrive(e, backupDataJson);
                }
                finally
                {
                    taskProgressService.FinishProgress();
                }
            }
            return false;
        }

        private void InitializeRouteData(BackupData backupData)
        {
            foreach (var route in backupData.Routes)
            {
                routeService.InitializeRouteData(route);
            }
        }

        private string ConvertToJson(BackupData backupData)
        {
            var backupDataJson = JsonConvert.SerializeObject(backupData, new JsonSerializerSettings()
            {
                ContractResolver = new WritablePropertiesOnlyResolver()
            });
            return backupDataJson;
        }

        private async Task<bool> WriteToFileInOneDrive(Event e, string backupDataJson)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(backupDataJson)))
                {
                    var cancelToken = new CancellationToken();
                    
                    string fileName = string.Format("trip-'{0}'.utt", e.Name);
                    var liveClient = new LiveConnectClient(liveAccessToken.AccessToken);
                    await InitializeFolderUploadUrl(liveClient);
                    var uploadResult =
                        await liveClient.UploadAsync(folderUploadUrl, fileName, stream, OverwriteOption.Overwrite,
                                cancelToken, new OneDriveProgressHandler("Saving trip to file...", taskProgressService));
                    return uploadResult.Result != null;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<TripRestoreResult> Restore()
        {
            var currentUser = userService.GetCurrentUser();
            var result = new TripRestoreResult();
            if (currentUser != null && liveAccessToken.AccessToken != null)
            {
                try
                {
                    taskProgressService.UpdateProgress(1, 2, "Start to restore trips...");
                    var liveClient = new LiveConnectClient(liveAccessToken.AccessToken);

                    await InitializeFolderUploadUrl(liveClient);
                    var request = new OneDriveFileRequest(folderUploadUrl, (name) =>
                    {
                        Regex regex = new Regex(FILE_NAME_PATTERN);
                        Match match = regex.Match(name);
                        return match.Success;
                    }, liveAccessToken.AccessToken);

                    taskProgressService.UpdateProgress(1, 2, "Getting all back-up files of trips...");
                    var files = await oneDriveFileExplorerService.GetFiles(request);

                    await RestoreTripsFromFiles(currentUser, result, files);
                }
                finally
                {
                    taskProgressService.FinishProgress();
                }
            }
            return result;
        }

        private async Task RestoreTripsFromFiles(User currentUser, TripRestoreResult result, ICollection<UTravellerModel.UTraveller.Model.File> files)
        {
            int progress = 1;
            foreach (var file in files)
            {
                if (!file.IsDirectory && !taskProgressService.IsCanceled)
                {
                    try
                    {
                        taskProgressService.UpdateProgress(progress++, files.Count, string.Format("Restore trip from file {0}", file.Name));
                        var fileContent =
                            await oneDriveFileExplorerService.ReadFile(new OneDriveFileRequest(file.Id, liveAccessToken.AccessToken));
                        using (var stream = new StreamReader(fileContent))
                        {
                            var streamContent = await stream.ReadToEndAsync();
                            if (streamContent != null)
                            {
                                var backupData = JsonConvert.DeserializeObject<BackupData>(streamContent);
                                await RestoreTripAndAllRelatedContent(currentUser, backupData, result);
                                result.RestoredFilenames.Add(file.Name);
                            }
                        }
                    }
                    catch (LimitExceedException le)
                    {
                        result.TripsRestoredWithWarnings.Add(new FailedTripResult(file.Name, le.Message));
                    }
                    catch (Exception ex)
                    {
                        result.FailedTrips.Add(new FailedTripResult(file.Name, ex.Message));
                    }
                }
            }
        }

        private async Task RestoreTripAndAllRelatedContent(User currentUser, BackupData backupData, TripRestoreResult restoreResult)
        {
            var trip = backupData.Trip;

            if (!eventService.IsEventNameExist(trip.Name, userService.GetCurrentUser()))
            {
                eventService.AddEvent(trip, currentUser);

                await RestoreTripEntity(async () =>
                {
                    foreach (var photo in backupData.Photos)
                    {
                        await photoService.AddPhoto(photo, trip);
                    }
                }, restoreResult, trip);

                await RestoreTripEntity(async () =>
                {
                    foreach (var message in backupData.Messages)
                    {
                        await messageService.AddMessageToEvent(message, trip);
                    }
                }, restoreResult, trip);

                await RestoreTripEntity(async () =>
                {
                    foreach (var expense in backupData.Expenses)
                    {
                        moneySpendingService.AddExpense(expense, trip);
                    }
                }, restoreResult, trip);

                await RestoreTripEntity(async () =>
                {
                    foreach (var route in backupData.Routes)
                    {
                        routeService.AddRoute(route, trip);
                    }
                }, restoreResult, trip);

                tripPlanService.AddTripPlan(backupData.TripPlan, trip);
            }
            else
            {
                throw new TripBackupException(string.Format("Trip with name {0} already exist", trip.Name));
            }
        }

        private async Task RestoreTripEntity(RestoreTripContent restoreTripContent, TripRestoreResult restoreResult, Event trip)
        {
            try
            {
                await restoreTripContent();
            }
            catch (LimitExceedException le)
            {
                restoreResult.TripsRestoredWithWarnings.Add(new FailedTripResult(trip.Name, le.Message));
            }
        }

        private async Task InitializeFolderUploadUrl(LiveConnectClient liveClient)
        {
            if (folderUploadUrl == null)
            {
                LiveOperationResult operationResult = await liveClient.GetAsync("/me/skydrive/files");

                var data = (List<object>)operationResult.Result["data"];
                foreach (IDictionary<string, object> content in data)
                {
                    var isDirectory = content["type"].Equals("folder") || content["type"].Equals("album") ? true : false;
                    var fileName = (string)content["name"];
                    var uploadLocation = (string)content["id"];
                    if (isDirectory && fileName.Equals(DEFAULT_ALBUM_NAME))
                    {
                        folderUploadUrl = uploadLocation;
                        break;
                    }
                }
                if (data.Count > 0 && folderUploadUrl == null)
                {
                    var folderData = new Dictionary<string, object>();
                    folderData.Add("name", DEFAULT_ALBUM_NAME);
                    var result = await liveClient.PostAsync("me/skydrive", folderData);
                    folderUploadUrl = result.Result["id"].ToString();
                }
            }
        }
    }

    delegate Task RestoreTripContent();

    class OneDriveProgressHandler : IProgress<LiveOperationProgress>
    {
        private ICancelableTaskProgressService taskProgressService;
        private string progressMessage;

        public OneDriveProgressHandler(string progressMessage, ICancelableTaskProgressService taskProgressService)
        {
            this.progressMessage = progressMessage;
            this.taskProgressService = taskProgressService;
        }

        public void Report(LiveOperationProgress value)
        {
            taskProgressService.UpdateProgress((int)value.ProgressPercentage, 100, progressMessage, false);
        }

    }
}
