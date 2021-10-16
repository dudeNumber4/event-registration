using EventData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventRepository
{

    /// <summary>
    /// Used so unit tests can point to alternate location for data.
    /// </summary>
    public interface IDataPreparer
    {
        void Prepare() { /*default no action*/ }
    }

    public class TestDataPreparer : IDataPreparer
    {
        public string DataPath { get => DataDriver.DataPath; }
        public void Prepare()
        {
            DataDriver.Set(Path.GetTempPath());
            DataDriver.Flush();
        }
    }

    public class DefaultDataPreparer : IDataPreparer {}

}
