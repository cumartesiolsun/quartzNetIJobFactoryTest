using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzNetJobFactoryTest
{

    public class ItJob : IJob
    {
        private IlkDataContext _IlkContext;
        private IImWorkFlow _ImWorkFlow;
        private ILogManager _logManager;
        private bool _isRunning;

        public ItJob(IImWorkFlow ImWorkFlow, ILogManager logManager, IlkDataContext IlkContext)
        {
            _ImWorkFlow = ImWorkFlow;
            _logManager = logManager;
            _IlkContext = IlkContext;
        }

        private async Task Execute()
        {
            try
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    Console.WriteLine("_isRunning True " + DateTime.Now);
                    Go();
                    _isRunning = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await ExceptionLogAsync(ex);
            }
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            return Task.Run(Execute);
        }

        private void Go()
        {
            try
            {
               // todo 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ExceptionLogAsync(ex);
            }
        }

        public async Task ExceptionLogAsync(Exception exception)
        { 
        }
    }
}
