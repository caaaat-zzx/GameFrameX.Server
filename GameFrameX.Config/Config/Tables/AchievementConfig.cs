
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Core.Config;

namespace GameFrameX.Config.Tables
{
    public sealed partial class AchievementConfig : BeanBase
    {
        /*
        public AchievementConfig(int Id, int Image, string Name, string AchievementContent, string LockText, System.Collections.Generic.List<int> AchievementUnlockCondition) 
        {
            this.Id = Id;
            this.Image = Image;
            this.Name = Name;
            this.AchievementContent = AchievementContent;
            this.LockText = LockText;
            this.AchievementUnlockCondition = AchievementUnlockCondition;
            PostInit();
        }        
        */

        public AchievementConfig(JsonElement _buf) 
        {
            Id = _buf.GetProperty("id").GetInt32();
            Image = _buf.GetProperty("image").GetInt32();
            Name = _buf.GetProperty("name").GetString();
            AchievementContent = _buf.GetProperty("achievement_content").GetString();
            LockText = _buf.GetProperty("LockText").GetString();
            { var __json0 = _buf.GetProperty("achievement_unlock_condition"); AchievementUnlockCondition = new System.Collections.Generic.List<int>(__json0.GetArrayLength()); foreach(JsonElement __e0 in __json0.EnumerateArray()) { int __v0;  __v0 = __e0.GetInt32();  AchievementUnlockCondition.Add(__v0); }   }
        }
    
        public static AchievementConfig DeserializeAchievementConfig(JsonElement _buf)
        {
            return new Tables.AchievementConfig(_buf);
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 成就对应的图标id
        /// </summary>
        public int Image { private set; get; }
        /// <summary>
        /// 成就Key
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 成就内容Key
        /// </summary>
        public string AchievementContent { private set; get; }
        /// <summary>
        /// 未解锁文字key
        /// </summary>
        public string LockText { private set; get; }
        /// <summary>
        /// 成就解锁条件
        /// </summary>
        public System.Collections.Generic.List<int> AchievementUnlockCondition { private set; get; }

        private const int __ID__ = -1961757688;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(TablesComponent tables)
        {
            
            
            
            
            
            
        }

        public override string ToString()
        {
            return "{ "
            + "id:" + Id + ","
            + "image:" + Image + ","
            + "name:" + Name + ","
            + "achievementContent:" + AchievementContent + ","
            + "LockText:" + LockText + ","
            + "achievementUnlockCondition:" + StringUtil.CollectionToString(AchievementUnlockCondition) + ","
            + "}";
        }

        partial void PostInit();
    }
}
