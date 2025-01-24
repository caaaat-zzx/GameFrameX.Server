using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络通道
/// </summary>
public interface INetWorkChannel
{
    /// <summary>
    /// 应用会话对象
    /// </summary>
    IGameAppSession GameAppSession { get; }

    /// <summary>
    /// RPC 会话
    /// </summary>
    IRpcSession RpcSession { get; }

    /// <summary>
    /// 异步写入消息
    /// </summary>
    /// <param name="msg">消息对象</param>
    /// <param name="errorCode">错误码</param>
    /// <returns></returns>
    Task WriteAsync(INetworkMessage msg, int errorCode = 0);

    /// <summary>
    /// 关闭网络
    /// </summary>
    void Close();

    /// <summary>
    /// 获取用户数据对象.
    /// 可能会发生转换失败的异常。
    /// 如果数据不存在则返回null
    /// </summary>
    /// <param name="key">数据Key</param>
    /// <typeparam name="T">将要获取的数据类型。</typeparam>
    /// <returns>用户数据对象</returns>
    T GetData<T>(string key);

    /// <summary>
    /// 清除自定义数据
    /// </summary>
    void ClearData();

    /// <summary>
    /// 移除用户数据
    /// </summary>
    /// <param name="key">数据Key</param>
    void RemoveData(string key);

    /// <summary>
    /// 设置用户数据
    /// </summary>
    /// <param name="key">数据Key</param>
    /// <param name="value">数据值</param>
    void SetData(string key, object value);

    /// <summary>
    /// 更新接收消息时间
    /// </summary>
    /// <param name="offsetTicks"></param>
    void UpdateReceiveMessageTime(long offsetTicks = 0);

    /// <summary>
    /// 获取最后一次消息的时间
    /// </summary>
    /// <param name="utcTime">UTC时间</param>
    /// <returns></returns>
    long GetLastMessageTimeSecond(in DateTime utcTime);

    /// <summary>
    /// 网络是否已经关闭
    /// </summary>
    /// <returns>是否已经关闭</returns>
    bool IsClosed();
}