using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public interface IBaseEntity<T>
    {
        T Id { get; set; }
    }

    public interface IRemotableEntity<T> : IBaseEntity<T>
    {
        T RemoteId { get; set; }

        bool IsDeleted { get; set; }

        bool IsSync { get; set; }

        DateTime? ChangeDate { get; set; }
    }
}
