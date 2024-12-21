
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
    public abstract partial class RefDynamicBase : BeanBase
    {
        /*
        public RefDynamicBase(int X) 
        {
            this.X = X;
            this.X_Ref = null;
            PostInit();
        }        
        */

        public RefDynamicBase(JsonElement _buf) 
        {
            X = _buf.GetProperty("x").GetInt32();
            X_Ref = null;
        }
    
        public static RefDynamicBase DeserializeRefDynamicBase(JsonElement _buf)
        {
            switch (_buf.GetProperty("$type").GetString())
            {
                case "RefBean": return new test.RefBean(_buf);
                default: throw new SerializationException();
            }
        }

        public int X { private set; get; }
        public test.TestBeRef X_Ref { private set; get; }


        public virtual void ResolveRef(TablesComponent tables)
        {
            X_Ref = tables.TbTestBeRef.Get(X);
        }

        public override string ToString()
        {
            return "{ "
            + "x:" + X + ","
            + "}";
        }

        partial void PostInit();
    }
}