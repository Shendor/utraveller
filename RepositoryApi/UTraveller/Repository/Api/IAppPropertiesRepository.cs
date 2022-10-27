﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IAppPropertiesRepository : IBaseRepository<AppPropertiesEntity, long>
    {
        AppPropertiesEntity GetPropertiesForUser(long userId);
    }
}
