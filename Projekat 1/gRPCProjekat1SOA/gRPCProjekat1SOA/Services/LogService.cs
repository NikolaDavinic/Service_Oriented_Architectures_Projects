using Grpc.Core;
using gRPCProjekat1SOA.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NetworkSpace;

namespace gRPCProjekat1SOA.Services
{
    public class LogService : Log.LogBase
    {
        private readonly ILogger<LogService> _logger;
        private readonly LogDbContext _dbcontext;

        public LogService(ILogger<LogService> logger, LogDbContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }

        public override async Task GetAllLogs(EmptyMessage request, IServerStreamWriter<LogValues> responseStream, ServerCallContext context)
        {
            var logValues = await _dbcontext.LogVals.ToListAsync();
            foreach(var lv in logValues)
            {
                await responseStream.WriteAsync(new LogValues
                {
                    FrameNumber = lv.FrameNumber,
                    FrameLen = lv.FrameLen,
                    FrameTime = lv.FrameTime,
                    IpDst = lv.IpDst,
                    IpSrc = lv.IpSrc,
                    IpLen = lv.IpLen,
                    TcpLen = lv.TcpLen,
                    Value = lv.Value,
                    Normality = lv.Normality
                });
            }
        }

        public override async Task<LogValues> GetLogById(LogId logId, ServerCallContext context)
        {
            var lv = await _dbcontext.LogVals.FindAsync(logId.Framenumber);

            return await Task.FromResult(new LogValues
            {
                FrameNumber = lv.FrameNumber,
                FrameLen = lv.FrameLen,
                FrameTime = lv.FrameTime,
                IpDst = lv.IpDst,
                IpSrc = lv.IpSrc,
                IpLen = lv.IpLen,
                TcpLen = lv.TcpLen,
                Value = lv.Value,
                Normality = lv.Normality
            });
        }

        public override async Task<LogValues> AddLog(LogValues logValues, ServerCallContext context)
        {
            var lv = new LogVal()
            {
                FrameNumber = logValues.FrameNumber,
                FrameLen = logValues.FrameLen,
                FrameTime = logValues.FrameTime,
                IpDst = logValues.IpDst,
                IpSrc = logValues.IpSrc,
                IpLen = logValues.IpLen,
                TcpLen = logValues.TcpLen,
                Value = logValues.Value,
                Normality = logValues.Normality
            };

            await _dbcontext.LogVals.AddAsync(lv);
            await _dbcontext.SaveChangesAsync();

            return await Task.FromResult(new LogValues
            {
                FrameNumber = lv.FrameNumber,
                FrameLen = lv.FrameLen,
                FrameTime = lv.FrameTime,
                IpDst = lv.IpDst,
                IpSrc = lv.IpSrc,
                IpLen = lv.IpLen,
                TcpLen = lv.TcpLen,
                Value = lv.Value,
                Normality = lv.Normality
            });
        }

        public override async Task<LogValues> UpdateLog(LogValues logValues, ServerCallContext context)
        {
            var lv = await _dbcontext.LogVals.FindAsync(logValues.FrameNumber);
            lv.FrameLen = logValues.FrameLen;
            lv.FrameTime = logValues.FrameTime;
            lv.TcpLen = logValues.TcpLen;
            lv.IpSrc = logValues.IpSrc;
            lv.IpDst = logValues.IpDst;
            lv.Value = logValues.Value;
            lv.Normality = logValues.Normality;
            lv.IpLen = logValues.IpLen;

            _dbcontext.LogVals.Update(lv);
            await _dbcontext.SaveChangesAsync();

            return await Task.FromResult(new LogValues
            {
                FrameNumber = lv.FrameNumber,
                FrameLen = lv.FrameLen,
                FrameTime = lv.FrameTime,
                IpDst = lv.IpDst,
                IpSrc = lv.IpSrc,
                IpLen = lv.IpLen,
                TcpLen = lv.TcpLen,
                Value = lv.Value,
                Normality = lv.Normality
            });
        }

        public override async Task<EmptyMessage> DeleteLog(LogId logId, ServerCallContext context)
        {
            var lv = await _dbcontext.LogVals.FindAsync(logId.Framenumber);
            _dbcontext.LogVals.Remove(lv);
            await _dbcontext.SaveChangesAsync();

            return await Task.FromResult(new EmptyMessage { });
        }
    }
}
