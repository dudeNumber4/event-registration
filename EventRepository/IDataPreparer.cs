using EventData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventRepository
{

    public interface IDataPreparer
    {
        void Prepare() { /*default no action*/ }
    }

    public class TempDataPreparer : IDataPreparer
    {
        public void Prepare()
        {
            DataDriver.Set(Path.GetTempPath());
            DataDriver.Flush();
        }
    }
    
    public class DefaultDataPreparer : IDataPreparer {}

}
