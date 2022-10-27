using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Mapper
{
    public interface IModelMapper<M, E>
    {
        M MapEntity(E entity);

        E MapModel(M model);
    }
}
