using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class ObjectMessage<T>
    {
        public ObjectMessage()
        {
        }

        public ObjectMessage(T obj)
        {
            Object = obj;
        }

        public T Object
        {
            get;
            set;
        }
    }
}
