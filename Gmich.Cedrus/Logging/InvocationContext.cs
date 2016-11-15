using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.Logging
{
    public class InvocationContext
    {
        private readonly Dictionary<string, object> logInfoTable = new Dictionary<string, object>();
             
        public void AddEntry(string layout, string value)
        {
            if (!logInfoTable.ContainsKey(layout))
            {
                logInfoTable.Add(layout, value);
            }
        }

        public void AddEntry(string layout, object value)
        {
            if (!logInfoTable.ContainsKey(layout))
            {
                logInfoTable.Add(layout, value);
            }
        }

        public object this[string layout] => logInfoTable.ContainsKey(layout) ?
            logInfoTable[layout] : null;
    }
}
