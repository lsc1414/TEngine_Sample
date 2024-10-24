
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace GameConfig.l10n
{
public sealed partial class Localization : Luban.BeanBase
{
    public Localization(ByteBuf _buf) 
    {
        Key = _buf.ReadString();
        En = _buf.ReadString();
        Zh = _buf.ReadString();
        Jp = _buf.ReadString();
    }

    public static Localization DeserializeLocalization(ByteBuf _buf)
    {
        return new l10n.Localization(_buf);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public readonly string Key;
    /// <summary>
    /// 英文
    /// </summary>
    public readonly string En;
    /// <summary>
    /// 中文
    /// </summary>
    public readonly string Zh;
    /// <summary>
    /// 日文
    /// </summary>
    public readonly string Jp;
   
    public const int __ID__ = -805964668;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "key:" + Key + ","
        + "en:" + En + ","
        + "zh:" + Zh + ","
        + "jp:" + Jp + ","
        + "}";
    }
}

}

