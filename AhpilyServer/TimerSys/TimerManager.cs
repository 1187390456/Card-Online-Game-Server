using AhpilyServer.Concurrent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AhpilyServer.TimerSys
{
    /// <summary>
    /// 定时任务管理类
    /// </summary>
    public class TimerManager
    {
        private static TimerManager instance = null;

        public static TimerManager Instance
        {
            get
            {
                // 锁住自身实例 防止多个线程同时访问
                lock (instance)
                {
                    if (instance == null)
                    {
                        instance = new TimerManager();
                    }
                    return instance;
                }
            }
        }

        private Timer timer; // 实现定时器主要功能 Timer类

        /// <summary>
        /// 安全字典存储 任务id 和任务模型 映射
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModelDict = new ConcurrentDictionary<int, TimerModel>();

        /// <summary>
        ///  标识id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 需要移除的任务id列表
        /// </summary>
        private List<int> removeIdList = new List<int>();

        public TimerManager()
        {
            timer = new Timer(10); // 10毫秒触发
            timer.Elapsed += Timer_Elapsed; // 触发事件
        }

        /// <summary>
        /// 达到时间间隔时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (removeIdList) // 锁住区域 让其在不变的情况下执行
            {
                foreach (var id in removeIdList)
                {
                    idModelDict.TryRemove(id, out TimerModel model);
                }

                removeIdList.Clear();
            }

            foreach (var model in idModelDict.Values)
            {
                // 当前时间大于等于 指定时间(加了延时) 开始触发
                if (DateTime.Now.Ticks >= model.Time) model.TriggerTimeEvent();
            }
        }

        /// <summary>
        /// 添加定时任务 指定具体触发事件
        /// </summary>
        public void AddTimeEvent(DateTime dateTime, TimerDelegate timerDelegate)
        {
            long delayTime = dateTime.Ticks - DateTime.Now.Ticks; // 设定时间-当前时间的周期数   即时间延时
            if (delayTime <= 0) return; // 无效时间设置 即设置的时间比当前时间还早
            AddTimeEvent(delayTime, timerDelegate);
        }

        /// <summary>
        /// 添加定义任务 指定延时多少执行
        /// </summary>
        /// <param name="delayTime">毫秒</param>
        /// <param name="timerDelegate"></param>
        public void AddTimeEvent(long delayTime, TimerDelegate timerDelegate)
        {
            TimerModel timerModel = new TimerModel(id.Add_Get(), DateTime.Now.Ticks + delayTime, timerDelegate);
            idModelDict.TryAdd(timerModel.Id, timerModel);
        }
    }
}