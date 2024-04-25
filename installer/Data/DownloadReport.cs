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
                bFTE = value;
                OnPropertyChanged();
            }
        }

        // 下载文件数量
        public long Count
        {
            get => cot; set
            {
                cot = value;
                OnPropertyChanged();
            }
        }

        // 下载完成的文件数量
        public long ComCount
        {
            get => com; set
            {
                com = value;
                OnPropertyChanged();
            }
        }

        // 文件大小
        public long Total
        {
            get => tot; set
            {
                tot = value;
                OnPropertyChanged();
            }
        }

        // 已完成大小
        public long Completed
        {
            get => cop; set
            {
                cop = value;
                OnPropertyChanged();
            }
        }

        private bool bFTE;
        private long cot;
        private long com;
        private long tot;
        private long cop;
    }
}
