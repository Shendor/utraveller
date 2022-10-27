using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IAppPropertiesService
    {
        AppProperties GetPropertiesForUser(long userId);

        Task RefreshPropertiesFromServer(User user);

        void UpdateProperties(AppProperties appProperties, User user);

        Task<bool> IsVersionUpToDate(string version);
    }
}
