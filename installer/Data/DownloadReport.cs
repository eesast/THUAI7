using installer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (bFTE != value) OnPropertyChanged();
                bFTE = value;
            }
        }

        // 下载文件数量
        public long Count
        {
            get => cot; set
            {
                if (cot != value) OnPropertyChanged();
                cot = value;
            }
        }

        // 下载完成的文件数量
        public long ComCount
        {
            get => com; set
            {
                if (com != value) OnPropertyChanged();
                com = value;
            }
        }

        // 文件大小
        public long Total
        {
            get => tot; set
            {
                if (tot != value) OnPropertyChanged();
                tot = value;
            }
        }

        // 已完成大小
        public long Completed
        {
            get => cop; set
            {
                if (cop != value) OnPropertyChanged();
                cop = value;
            }
        }

        private bool bFTE;
        private long cot;
        private long com;
        private long tot;
        private long cop;
    }
}
