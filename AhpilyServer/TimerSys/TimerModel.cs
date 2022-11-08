using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer.TimerSys
{
    /// <summary>
    /// 当定时器达到时间触发
    /// </summary>
    public delegate void TimerDelegate();

    /// <summary>
    /// 定时器任务的数据模型
    /// </summary>
    public class TimerModel
    {
        public int Id; // 标识id

        /// <summary>
        /// 任务执行时间
        /// </summary>
        public long Time;

        private TimerDelegate TimeDelegate;

        public TimerModel(int id, long time, TimerDelegate timeDelegate)
        {
            Id = id;
            Time = time;
            TimeDelegate = timeDelegate;
        }

        /// <summary>
        /// 触发定时任务
        /// </summary>
        public void TriggerTimeEvent()
        {
            TimeDelegate();
        }
    }
}