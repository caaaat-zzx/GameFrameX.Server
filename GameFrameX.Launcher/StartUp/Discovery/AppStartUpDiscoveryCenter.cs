using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.ServerManager;


namespace GameFrameX.Launcher.StartUp.Discovery;

/// <summary>
/// 服务发现中心服务器
/// </summary>
[StartUpTag(ServerType.DiscoveryCenter, 0)]
internal partial class AppStartUpDiscoveryCenter : AppStartUpService
{
    public override async Task StartAsync()
    {
        try
        {
            _namingServiceManager.AddSelf(Setting);

            await StartServer();

            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info($"服务器{ServerType}执行异常，e:{e}");
            LogHelper.Fatal(e);
        }

        await StopAsync();
    }

    /// <summary>
    /// 发送消息给注册的服务
    /// </summary>
    /// <param name="session">连接会话对象</param>
    /// <param name="message">消息对象</param>
    private async void SendMessage(IAppSession session, INetworkMessage message)
    {
        message.CheckNotNull(nameof(message));
        if (session == null || session.Connection.IsClosed)
        {
            return;
        }

        MessageProtoHelper.SetMessageIdAndOperationType(message);
        var messageObjectHeader = new InnerMessageObjectHeader()
        {
            ServerId = Setting.ServerId,
        };
        var innerNetworkMessage = InnerNetworkMessage.Create(message, messageObjectHeader);
        var buffer = MessageEncoderHandler.Handler(innerNetworkMessage);
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            var serverInfo = _namingServiceManager.GetNodeBySessionId(session.SessionID);
            if (serverInfo != null)
            {
                LogHelper.Info("---发送[" + ServerType + " To " + serverInfo.Type + "]  " + innerNetworkMessage.ToFormatMessageString());
            }
        }

        await session.SendAsync(buffer);
    }

    protected override ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            var serverInfo = _namingServiceManager.GetNodeBySessionId(session.SessionID);
            if (serverInfo != null)
            {
                LogHelper.Debug($"---收到[{serverInfo.Type} To {ServerType}]  {message.ToFormatMessageString()}");
            }
            else
            {
                LogHelper.Debug($"---收到[{ServerType}]  {message.ToFormatMessageString()}");
            }
        }

        if (message is IInnerNetworkMessage messageObject)
        {
            switch ((MessageOperationType)messageObject.Header.OperationType)
            {
                case MessageOperationType.None:
                    break;
                case MessageOperationType.HeartBeat:
                {
                    // 心跳响应
                    var reqHeartBeat = messageObject.DeserializeMessageObject();
                    var response = new NotifyHeartBeat()
                    {
                        UniqueId = reqHeartBeat.UniqueId,
                        Timestamp = TimeHelper.UnixTimeMilliseconds(),
                    };
                    SendMessage(session, response);
                    return ValueTask.CompletedTask;
                }
                case MessageOperationType.Cache:
                    break;
                case MessageOperationType.Database:
                    break;
                case MessageOperationType.Game:
                {
                    ReqConnectServer reqConnectServer = (ReqConnectServer)messageObject.DeserializeMessageObject();
                    var serverList = _namingServiceManager.GetNodesByType(reqConnectServer.ServerType);
                    if (reqConnectServer.ServerID > 0)
                    {
                        serverList = serverList.Where(m => m.ServerId == reqConnectServer.ServerID).ToList();
                    }

                    if (serverList.Count > 0)
                    {
                        var serverInfo = (ServiceInfo)serverList.Random();

                        RespConnectServer respConnectServer = new RespConnectServer
                        {
                            UniqueId = reqConnectServer.UniqueId,
                            ServerType = serverInfo.Type,
                            ServerName = serverInfo.ServerName,
                            ServerID = serverInfo.ServerId,
                            TargetIP = serverInfo.OuterIp,
                            TargetPort = serverInfo.OuterPort
                        };
                        SendMessage(session, respConnectServer);
                    }
                }
                    break;
                case MessageOperationType.GameManager:
                    break;
                case MessageOperationType.Forbid:
                    break;
                case MessageOperationType.Reboot:
                    break;
                case MessageOperationType.Reconnect:
                    break;
                case MessageOperationType.Reload:
                    break;
                case MessageOperationType.Exit:
                    break;
                case MessageOperationType.Kick:
                    break;
                case MessageOperationType.Notify:
                    break;
                case MessageOperationType.Forward:
                    break;
                case MessageOperationType.Register:
                {
                    ReqRegisterServer reqRegisterServer = (ReqRegisterServer)messageObject.DeserializeMessageObject();
                    // 注册服务
                    ServiceInfo serviceInfo = new ServiceInfo(reqRegisterServer.ServerType, session, session.SessionID, reqRegisterServer.ServerName, reqRegisterServer.ServerID, reqRegisterServer.InnerIP, reqRegisterServer.InnerPort, reqRegisterServer.OuterIP, reqRegisterServer.OuterPort);
                    _namingServiceManager.Add(serviceInfo);
                    LogHelper.Info($"注册服务成功：{reqRegisterServer.ServerType}  {reqRegisterServer.ServerName}  {reqRegisterServer}");
                    return ValueTask.CompletedTask;
                }
                case MessageOperationType.RequestConnectServer:
                {
                    ReqConnectServer reqConnectServer = (ReqConnectServer)messageObject.DeserializeMessageObject();
                    var serverList = _namingServiceManager.GetNodesByType(reqConnectServer.ServerType);
                    if (reqConnectServer.ServerId > 0)
                    {
                        serverList = serverList.Where(m => m.ServerId == reqConnectServer.ServerId).ToList();
                    }

                    RespConnectServer respConnectServer = new RespConnectServer
                    {
                        UniqueId = reqConnectServer.UniqueId,
                    };
                    if (serverList.Count > 0)
                    {
                        var serverInfo = (ServiceInfo)serverList.Random();
                        respConnectServer.ServerType = serverInfo.Type;
                        respConnectServer.ServerName = serverInfo.ServerName;
                        respConnectServer.ServerId = serverInfo.ServerId;
                        respConnectServer.TargetIp = serverInfo.OuterIp;
                        respConnectServer.TargetPort = serverInfo.OuterPort;
                    }

                    SendMessage(session, respConnectServer);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return ValueTask.CompletedTask;
    }


    protected override ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部服务连接到中心服务器成功" + "。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs args)
    {
        LogHelper.Info("有外部服务从中心服务器断开。链接信息：断开原因:" + args.Reason);
        _namingServiceManager.TrySessionRemove(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    protected override void ConfigureSuperSocket(ServerOptions options)
    {
        options.ClearIdleSessionInterval = 30;
        base.ConfigureSuperSocket(options);
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 21000,
                ServerType = ServerType.DiscoveryCenter,
                InnerPort = 21001,
                APMPort = 21090,
                IsDebug = true,
                IsDebugReceive = true,
                IsDebugSend = true
            };
        }

        base.Init();
    }
}