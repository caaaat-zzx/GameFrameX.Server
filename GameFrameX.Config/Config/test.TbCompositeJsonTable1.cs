
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
    public partial class TbCompositeJsonTable1 : BaseDataTable<test.CompositeJsonTable1>
    {
        //private readonly System.Collections.Generic.Dictionary<int, test.CompositeJsonTable1> _dataMap;
        //private readonly System.Collections.Generic.List<test.CompositeJsonTable1> _dataList;
    
        //public System.Collections.Generic.Dictionary<int, test.CompositeJsonTable1> DataMap => _dataMap;
        //public System.Collections.Generic.List<test.CompositeJsonTable1> DataList => _dataList;
        //public test.CompositeJsonTable1 GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
        //public test.CompositeJsonTable1 Get(int key) => _dataMap[key];
        //public test.CompositeJsonTable1 this[int key] => _dataMap[key];
    
        public override async System.Threading.Tasks.Task LoadAsync()
        {
            var jsonElement = await _loadFunc();
            DataList.Clear();
            LongDataMaps.Clear();
            StringDataMaps.Clear();
            foreach(var element in jsonElement.EnumerateArray())
            {
                test.CompositeJsonTable1 _v;
                _v = test.CompositeJsonTable1.DeserializeCompositeJsonTable1(element);
                DataList.Add(_v);
                LongDataMaps.Add(_v.Id, _v);
                StringDataMaps.Add(_v.Id.ToString(), _v);
            }
            PostInit();
        }

        public void ResolveRef(TablesComponent tables)
        {
            foreach(var element in DataList)
            {
                element.ResolveRef(tables);
            }
        }
    
    
        partial void PostInit();

        public TbCompositeJsonTable1(Func<Task<JsonElement>> loadFunc) : base(loadFunc)
        {
        }
    }
}