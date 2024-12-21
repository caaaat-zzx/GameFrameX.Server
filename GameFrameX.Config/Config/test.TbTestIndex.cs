
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
    public partial class TbTestIndex : BaseDataTable<test.TestIndex>
    {
        //private readonly System.Collections.Generic.Dictionary<int, test.TestIndex> _dataMap;
        //private readonly System.Collections.Generic.List<test.TestIndex> _dataList;
    
        //public System.Collections.Generic.Dictionary<int, test.TestIndex> DataMap => _dataMap;
        //public System.Collections.Generic.List<test.TestIndex> DataList => _dataList;
        //public test.TestIndex GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
        //public test.TestIndex Get(int key) => _dataMap[key];
        //public test.TestIndex this[int key] => _dataMap[key];
    
        public override async System.Threading.Tasks.Task LoadAsync()
        {
            var jsonElement = await _loadFunc();
            DataList.Clear();
            LongDataMaps.Clear();
            StringDataMaps.Clear();
            foreach(var element in jsonElement.EnumerateArray())
            {
                test.TestIndex _v;
                _v = test.TestIndex.DeserializeTestIndex(element);
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

        public TbTestIndex(Func<Task<JsonElement>> loadFunc) : base(loadFunc)
        {
        }
    }
}