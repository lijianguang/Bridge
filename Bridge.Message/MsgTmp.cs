using B.M;

namespace Bridge.Message
{
    public class MsgTmp
    {
        public string? Name {  get; set; }
        public int Age {  get; set; }
        public MsgTmp1 MsgTmp1 { get; set; }
    }
    public class MsgTmp1
    {
        public MsgTmp2 MsgTmp2 { get; set; }
    }
}
namespace B.M
{
    public class MsgTmp2
    {
        public int Age { get; set; }
    }
}
