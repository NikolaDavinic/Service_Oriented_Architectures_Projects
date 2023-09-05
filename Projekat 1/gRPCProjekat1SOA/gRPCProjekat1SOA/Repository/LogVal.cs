using System.ComponentModel.DataAnnotations;

namespace gRPCProjekat1SOA.Repository
{
    public class LogVal
    {
        [Key]
        public int FrameNumber { get; set; }
        public int FrameLen { get; set; }
        public double FrameTime { get; set; }
        public int IpSrc { get; set; }
        public int IpDst { get; set; }
        public int IpLen { get; set; }
        public int TcpLen { get; set; }
        public int Value { get; set; }
        public int Normality { get; set; }
    }
}
