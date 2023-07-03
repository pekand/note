using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note
{
    public class FileConfig
    {
        public string filePath = "";
        public bool isSaved = false;
        public bool isOpen = false;
        public string content = "";
        public bool autosave = false;
        public bool ontop = false;
        public bool showmenu = false;
        public bool transparent = false;
        public Color color = Color.Gray;
        public Color background = Color.Black;
    }
}
