
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.test
{
    public sealed partial class DateTimeRange : BeanBase
    {
        /*
        public DateTimeRange(long StartTime, long EndTime) 
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            PostInit();
        }        
        */

        public DateTimeRange(JsonElement _buf) 
        {
            StartTime = _buf.GetProperty("start_time").GetInt64();
            EndTime = _buf.GetProperty("end_time").GetInt64();
        }
    
        public static DateTimeRange DeserializeDateTimeRange(JsonElement _buf)
        {
            return new test.DateTimeRange(_buf);
        }

        public long StartTime { private set; get; }
        public long EndTime { private set; get; }

        private const int __ID__ = 495315430;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(TablesComponent tables)
        {
            
            
        }

        public override string ToString()
        {
            return "{ "
            + "startTime:" + StartTime + ","
            + "endTime:" + EndTime + ","
            + "}";
        }

        partial void PostInit();
    }
}