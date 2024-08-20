﻿using GameFrameX.Launcher.Common.Event;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Events;

namespace GameFrameX.Launcher.Common.Session
{
    /// <summary>
    /// 管理玩家session，一个玩家一个，下线之后移除，顶号之后释放之前的channel，替换channel
    /// </summary>
    public static class SessionManager
    {
        private static readonly ConcurrentDictionary<string, Session> SessionMap = new ConcurrentDictionary<string, Session>();

        /// <summary>
        /// 玩家数量
        /// </summary>
        /// <returns></returns>
        public static int Count()
        {
            return SessionMap.Count;
        }

        /// <summary>
        /// 获取分页玩家列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        public static List<Session> GetPageList(int pageSize, int pageIndex)
        {
            var result = SessionMap.Values.OrderBy(m => m.CreateTime).Where(m => ActorManager.HasActor(m.RoleId)).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// 踢掉玩家
        /// </summary>
        /// <param name="roleId">链接ID</param>
        public static void KickOffLineByUserId(long roleId)
        {
            var roleSession = SessionMap.Values.FirstOrDefault(m => m.RoleId == roleId);
            if (roleSession != null)
            {
                if (SessionMap.TryRemove(roleSession.Id, out var value) && ActorManager.HasActor(roleSession.RoleId))
                {
                    EventDispatcher.Dispatch(roleSession.RoleId, (int)EventId.SessionRemove);
                }
            }
        }

        /// <summary>
        /// 获取连接会话
        /// </summary>
        /// <param name="sessionId">链接ID</param>
        public static Session Get(string sessionId)
        {
            SessionMap.TryRemove(sessionId, out var value);
            return value;
        }

        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="sessionId">链接ID</param>
        public static Session Remove(string sessionId)
        {
            if (SessionMap.TryRemove(sessionId, out var value) && ActorManager.HasActor(value.RoleId))
            {
                EventDispatcher.Dispatch(value.RoleId, (int)EventId.SessionRemove);
            }

            return value;
        }

        /// <summary>
        /// 移除全部
        /// </summary>
        /// <returns></returns>
        public static Task RemoveAll()
        {
            foreach (var session in SessionMap.Values)
            {
                if (ActorManager.HasActor(session.RoleId))
                {
                    EventDispatcher.Dispatch(session.RoleId, (int)EventId.SessionRemove);
                }
            }

            SessionMap.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static INetWorkChannel GetChannel(string sessionId)
        {
            SessionMap.TryGetValue(sessionId, out var session);
            return session?.WorkChannel;
        }

        /// <summary>
        /// 添加新连接
        /// </summary>
        /// <param name="session"></param>
        public static void Add(Session session)
        {
            if (SessionMap.TryGetValue(session.Id, out var oldSession) && oldSession.WorkChannel != session.WorkChannel)
            {
                if (oldSession.Sign != session.Sign)
                {
                    var msg = new RespPrompt
                    {
                        Type = 5,
                        Content = "你的账号已在其他设备上登陆"
                    };
                    oldSession.WriteAsync(msg);
                }

                // 新连接 or 顶号
                oldSession.WorkChannel.RemoveData(GlobalConst.SessionIdKey);
                oldSession.WorkChannel.Close();
            }

            session.WorkChannel.SetData(GlobalConst.SessionIdKey, session.Id);
            SessionMap[session.Id] = session;
        }
    }
}