using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface ITripBackupService
    {
        Task<bool> Backup(Event e);

        Task<TripRestoreResult> Restore();
    }
}
