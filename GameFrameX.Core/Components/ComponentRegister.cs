﻿using System.Reflection;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Utility;
using GameFrameX.Extension;
using GameFrameX.Log;

namespace GameFrameX.Core.Components
{
    /// <summary>
    /// 组件注册器
    /// </summary>
    public static class ComponentRegister
    {
        /// <summary>
        /// ActorType -> CompTypeList
        /// </summary>
        private static readonly Dictionary<ActorType, HashSet<Type>> ActorComponentDic = new();

        /// <summary>
        /// CompType -> ActorType
        /// </summary>
        internal static readonly Dictionary<Type, ActorType> ComponentActorDic = new();

        /// <summary>
        /// func -> CompTypes
        /// </summary>
        private static readonly Dictionary<int, HashSet<Type>> FuncComponentDic = new();

        /// <summary>
        /// CompType -> func
        /// </summary>
        private static readonly Dictionary<Type, short> ComponentFuncDic = new();

        /// <summary>
        /// 根据CompType获取对应的ActorType类型
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public static ActorType GetActorType(Type compType)
        {
            ComponentActorDic.TryGetValue(compType, out var actorType);
            return actorType;
        }

        /// <summary>
        /// 根据ActorType类型获取对应的CompTypes列表
        /// </summary>
        /// <param name="actorType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetComponents(ActorType actorType)
        {
            ActorComponentDic.TryGetValue(actorType, out var comps);
            return comps;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <returns></returns>
        /// <exception cref="Exception">当程序集为null时抛出</exception>
        public static Task Init(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }

            assembly.CheckNotNull(nameof(assembly));
            var baseCompName = typeof(BaseComponent);
            var types        = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsAbstract || !type.IsSubclassOf(baseCompName))
                {
                    continue;
                }

                if (type.GetCustomAttribute(typeof(ComponentTypeAttribute)) is ComponentTypeAttribute compAttr)
                {
                    var actorType = compAttr.ActorType;
                    var compTypes = ActorComponentDic.GetOrAdd(actorType);
                    compTypes.Add(type);

                    ComponentActorDic[type] = actorType;

                    if (actorType == ActorType.Player)
                    {
                        if (type.GetCustomAttribute(typeof(FuncAttribute)) is FuncAttribute funcAttr)
                        {
                            var set = FuncComponentDic.GetOrAdd(funcAttr.Func);
                            set.Add(type);
                            ComponentFuncDic[type] = funcAttr.Func;
                        }
                    }
                }
                else
                {
                    throw new Exception($"comp:{type.FullName}未绑定actor类型");
                }
            }

            LogHelper.Info($"初始化组件注册完成");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 激活全局组件
        /// </summary>
        public static async Task ActiveGlobalComponents()
        {
            try
            {
                foreach (var (actorType, value) in ActorComponentDic)
                {
                    foreach (var compType in value)
                    {
                        var agentType = HotfixManager.GetAgentType(compType);
                        if (agentType == null)
                        {
                            LogHelper.Info($"{compType}未实现agent");
                        }

                        // if (actorType > ActorType.Separator)
                        // {
                        //     Log.Info($"激活全局组件：{actorType} {compType}");
                        //     await ActorMgr.GetCompAgent(agentType, actorType);
                        // }
                    }

                    if (actorType > ActorType.Separator)
                    {
                        LogHelper.Info($"激活全局Actor: {actorType}");
                        await ActorManager.GetOrNew(ActorIdGenerator.GetActorId(actorType));
                    }
                }

                LogHelper.Info($"激活全局组件并检测组件是否都包含Agent实现完成");
            }
            catch (Exception)
            {
                LogHelper.Error($"激活全局组件并检测组件是否都包含Agent实现失败");
                throw;
            }
        }

        /// <summary>
        /// 激活角色组件
        /// </summary>
        /// <param name="componentAgent"></param>
        /// <param name="openFuncSet"></param>
        /// <returns></returns>
        public static Task ActiveRoleComponents(IComponentAgent componentAgent, HashSet<short> openFuncSet)
        {
            return ActiveComponents(componentAgent.Owner.Actor,
                                    t => !ComponentFuncDic.TryGetValue(t, out var func)
                                         || openFuncSet.Contains(func));
            //foreach (var compType in GetComps(ActorType.Role))
            //{
            //    bool active;
            //    if (CompFuncDic.TryGetValue(compType, out var func))
            //    {
            //        active = openFuncSet.Contains(func);
            //    }
            //    else
            //    {
            //        active = true;
            //    }
            //    if (active)
            //    {
            //        var agentType = HotfixMgr.GetAgentType(compType);
            //        await compAgent.GetCompAgent(agentType);
            //    }
            //}
        }

        internal static async Task ActiveComponents(IActor actor, Func<Type, bool> predict = null)
        {
            var compTypes = GetComponents(actor.Type);

            if (compTypes != null)
            {
                foreach (var compType in compTypes)
                {
                    if (predict == null || predict(compType))
                    {
                        var agentType = HotfixManager.GetAgentType(compType);
                        try
                        {
                            await actor.GetComponentAgent(agentType);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Info(e);
                            // throw;
                        }
                    }
                }
            }
            else
            {
                LogHelper.Fatal($"获取不属于此actor：{actor.Type}的组件");
            }
        }

        internal static BaseComponent CreateComponent(Actor actor, Type compType)
        {
            if (!ActorComponentDic.TryGetValue(actor.Type, out var compTypes))
            {
                throw new Exception($"获取不属于此actor：{actor.Type}的comp:{compType.FullName}");
            }

            if (!compTypes.Contains(compType))
            {
                throw new Exception($"获取不属于此actor：{actor.Type}的comp:{compType.FullName}");
            }

            var comp = (BaseComponent)Activator.CreateInstance(compType);
            if (comp != null)
            {
                comp.Actor = actor;
                return comp;
            }

            return default;
        }
    }
}