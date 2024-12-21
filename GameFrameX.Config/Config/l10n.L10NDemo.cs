
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.l10n
{
    public sealed partial class L10NDemo : BeanBase
    {
        /*
        public L10NDemo(int Id, string Text) 
        {
            this.Id = Id;
            this.Text = Text;
            PostInit();
        }        
        */

        public L10NDemo(JsonElement _buf) 
        {
            Id = _buf.GetProperty("id").GetInt32();
            Text = _buf.GetProperty("text").GetString();
        }
    
        public static L10NDemo DeserializeL10NDemo(JsonElement _buf)
        {
            return new l10n.L10NDemo(_buf);
        }

        public int Id { private set; get; }
        public string Text { private set; get; }

        private const int __ID__ = -331195887;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(TablesComponent tables)
        {
            
            
        }

        public override string ToString()
        {
            return "{ "
            + "id:" + Id + ","
            + "text:" + Text + ","
            + "}";
        }

        partial void PostInit();
    }
}