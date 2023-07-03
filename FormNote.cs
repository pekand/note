using System.Xml;
using static System.Environment;

namespace Notepad
{
    public partial class Note : Form
    {
        public string filePath = "";
        public bool isSaved = false;
        public bool isOpen = false;
        public string content = "";
        public bool autosave = false;
        public bool ontop = false;

        public Note()
        {
            InitializeComponent();
        }

        private void Notepad_Load(object sender, EventArgs e)
        {
            this.loadConfig();

            string[] args = Environment.GetCommandLineArgs();

#if DEBUG
            args = new[] { "", "c:\\Users\\pekar\\Desktop\\test.txt" };
#endif

            if (args != null && args.Length > 1 && File.Exists(args[1]))
            {
                this.open(args[1]);
            }
        }



        public void newFile()
        {
            this.textBox.Text = "";
            this.content = "";
            this.filePath = "";
            this.isSaved = false;
            this.isOpen = false;
            this.Text = "Note";
        }

        public void saveAs()
        {
            this.save(true);
        }

        public void save(bool saveAs = false)
        {
            if (this.isOpen && !this.isSaved)
            {
                this.content = textBox.Text;
                File.WriteAllText(this.filePath, this.content);
                this.isSaved = true;
            }
        }

        public void open(string filePath = "")
        {

            if (File.Exists(filePath))
            {
                this.content = File.ReadAllText(filePath);
                this.textBox.Text = content;
                this.filePath = filePath;
                this.isSaved = true;
                this.isOpen = true;
                this.Text = this.filePath;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.newFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = this.openFileDialog.FileName;
                this.open(filePath);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = this.filePath;
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.saveFileDialog.FileName;
                this.Text = filePath;
                this.isSaved = false;
                this.isOpen = true;
                this.saveAs();
            }
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            this.ontop = this.TopMost;
            topToolStripMenuItem.Checked = this.TopMost;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textBox.Font;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Font = fontDialog.Font;
            }
        }

        private void autosaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autosave = !this.autosave;
            timer.Enabled = this.autosave;
            autosaveToolStripMenuItem.Checked = this.autosave;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autosaveToolStripMenuItem.Checked = this.autosave;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.autosave)
            {
                return;
            }

            this.save();
        }


        private void textBox_TextChanged(object sender, EventArgs e)
        {

            string newContent = textBox.Text;
            if (newContent != content)
            {
                this.isSaved = false;
            }

            this.content = newContent;
        }



        public int StrToIntDef(string? s, int @default)
        {
            int number;
            if (int.TryParse(s, out number))
                return number;
            return @default;
        }

        public void saveConfig()
        {

            var commonpath = GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configPath = Path.Combine(commonpath, "Note\\config.xml");

            FileInfo fileInfo = new FileInfo(configPath);
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create(configPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Config");

                writer.WriteElementString("autosave", this.autosave ? "1" : "0");
                writer.WriteElementString("ontop", this.ontop ? "1" : "0");
                writer.WriteElementString("x", this.Left.ToString());
                writer.WriteElementString("y", this.Top.ToString());
                writer.WriteElementString("width", this.Width.ToString());
                writer.WriteElementString("height", this.Height.ToString());

                string fontString = $"{textBox.Font.FontFamily.Name},{textBox.Font.Size},{textBox.Font.Style}";

                writer.WriteElementString("font", fontString);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }


        }


        public void loadConfig()
        {
            var commonpath = GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configPath = Path.Combine(commonpath, "Note\\config.xml");

            if (!File.Exists(configPath))
            {
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configPath);

            XmlNode configNode = xmlDoc.SelectSingleNode("/Config");

            this.autosave = configNode.SelectSingleNode("autosave")?.InnerText == "1";
            this.autosaveToolStripMenuItem.Checked = this.autosave;

            this.ontop = configNode.SelectSingleNode("ontop")?.InnerText == "1";
            this.TopMost = this.ontop;
            this.topToolStripMenuItem.Checked = this.TopMost;

            string fontString = configNode.SelectSingleNode("font")?.InnerText;
            if (fontString != null && fontString.Length > 0)
            {
                try
                {
                    string[] fontParts = fontString.Split(',');
                    string fontFamily = fontParts[0];
                    float fontSize = float.Parse(fontParts[1]);
                    FontStyle fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), fontParts[2]);
                    Font font = new Font(fontFamily, fontSize, fontStyle);
                    textBox.Font = font;
                }
                catch (Exception)
                {

                }

            }

            try
            {
                this.Left = this.StrToIntDef(configNode.SelectSingleNode("x")?.InnerText, this.Left);
                this.Top = this.StrToIntDef(configNode.SelectSingleNode("y")?.InnerText, this.Top);
                this.Width = this.StrToIntDef(configNode.SelectSingleNode("width")?.InnerText, this.Width);
                this.Height = this.StrToIntDef(configNode.SelectSingleNode("height")?.InnerText, this.Height);
            }
            catch (Exception)
            {

            }




        }

        private void Notepad_Shown(object sender, EventArgs e)
        {

        }

        private void Notepad_Leave(object sender, EventArgs e)
        {

        }

        private void Notepad_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.saveConfig();
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}