using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class Link
    {
        private string name;
        private string url;
        public string Name
        {
            get => name;
            set => name = value;
        }
        public string Url
        {
            get => url;
            set => url = value;
        }
    }
}
