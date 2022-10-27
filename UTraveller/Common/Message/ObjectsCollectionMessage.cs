using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class ObjectsCollectionMessage<T>
    {
        public ObjectsCollectionMessage()
        {
        }

        public ObjectsCollectionMessage(ICollection<T> objects)
        {
            Objects = objects;
        }

        public ICollection<T> Objects
        {
            get;
            set;
        }
    }
}
