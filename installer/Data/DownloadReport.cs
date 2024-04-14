using installer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace installer.Data
{
    public class DownloadReport : NotificationObject
    {
        // 是否开启大文件跟踪
        public bool BigFileTraceEnabled
        {
            get => bFTE; set
            {
                mutex.WaitOne();
                bFTE = value;
                OnPropertyChanged();
                mutex.ReleaseMutex();
            }
        }

        // 下载文件数量
        public long Count
        {
            get => cot; set
            {
                mutex.WaitOne();
                cot = value;
                OnPropertyChanged();
                mutex.ReleaseMutex();
            }
        }

        // 下载完成的文件数量
        public long ComCount
        {
            get => com; set
            {
                mutex.WaitOne();
                com = value;
                OnPropertyChanged();
                mutex.ReleaseMutex();
            }
        }

        // 文件大小
        public long Total
        {
            get => tot; set
            {
                mutex.WaitOne();
                tot = value;
                OnPropertyChanged();
                mutex.ReleaseMutex();
            }
        }

        // 已完成大小
        public long Completed
        {
            get => cop; set
            {
                mutex.WaitOne();
                cop = value;
                OnPropertyChanged();
                mutex.ReleaseMutex();
            }
        }

        public event PropertyChangingEventHandler? OnReport;

        public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            OnReport?.Invoke(this, null);
            base.OnPropertyChanged(propertyName);
        }

        private bool bFTE;
        private long cot;
        private long com;
        private long tot;
        private long cop;
        private Mutex mutex = new Mutex();
    }
}
