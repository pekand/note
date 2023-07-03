using Note;
using System.Xml;
using static System.Environment;

namespace Notepad
{
    public partial class Note : Form
    {
        public FileConfig fileConfig = new FileConfig();

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
            this.fileConfig.content = "";
            this.fileConfig.filePath = "";
            this.fileConfig.isSaved = false;
            this.fileConfig.isOpen = false;
            this.Text = "Note";
        }

        public void saveAs()
        {
            this.save(true);
        }

        public void save(bool saveAs = false)
        {
            if (this.fileConfig.isOpen && !this.fileConfig.isSaved)
            {
                this.fileConfig.content = textBox.Text;
                File.WriteAllText(this.fileConfig.filePath, this.fileConfig.content);
                this.fileConfig.isSaved = true;
            }
        }

        public void open(string filePath = "")
        {

            if (File.Exists(filePath))
            {
                this.fileConfig.content = File.ReadAllText(filePath);
                this.textBox.Text = this.fileConfig.content;
                this.fileConfig.filePath = filePath;
                this.fileConfig.isSaved = true;
                this.fileConfig.isOpen = true;
                this.Text = this.fileConfig.filePath;
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
            this.saveFileDialog.FileName = this.fileConfig.filePath;
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.fileConfig.filePath = this.saveFileDialog.FileName;
                this.Text = this.fileConfig.filePath;
                this.fileConfig.isSaved = false;
                this.fileConfig.isOpen = true;
                this.saveAs();
            }
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            this.fileConfig.ontop = this.TopMost;
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
            this.fileConfig.autosave = !this.fileConfig.autosave;
            timer.Enabled = this.fileConfig.autosave;
            autosaveToolStripMenuItem.Checked = this.fileConfig.autosave;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autosaveToolStripMenuItem.Checked = this.fileConfig.autosave;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.fileConfig.autosave)
            {
                return;
            }

            this.save();
        }


        private void textBox_TextChanged(object sender, EventArgs e)
        {

            string newContent = textBox.Text;
            if (newContent != this.fileConfig.content)
            {
                this.fileConfig.isSaved = false;
            }

            this.fileConfig.content = newContent;
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

                writer.WriteElementString("autosave", this.fileConfig.autosave ? "1" : "0");
                writer.WriteElementString("ontop", this.fileConfig.ontop ? "1" : "0");
                writer.WriteElementString("showmenu", this.fileConfig.showmenu ? "1" : "0");
                writer.WriteElementString("transparent", this.fileConfig.transparent ? "1" : "0");
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

            this.fileConfig.autosave = configNode.SelectSingleNode("autosave")?.InnerText == "1";
            this.autosaveToolStripMenuItem.Checked = this.fileConfig.autosave;

            this.fileConfig.ontop = configNode.SelectSingleNode("ontop")?.InnerText == "1";
            this.TopMost = this.fileConfig.ontop;
            this.topToolStripMenuItem.Checked = this.TopMost;

            this.fileConfig.showmenu = configNode.SelectSingleNode("showmenu")?.InnerText == "1";
            this.showMenuToolStripMenuItem.Checked = this.fileConfig.showmenu;
            this.menuStrip.Visible = this.fileConfig.showmenu;

            this.fileConfig.transparent = configNode.SelectSingleNode("transparent")?.InnerText == "1";
            this.transparentToolStripMenuItem.Checked = this.fileConfig.transparent;
            this.Opacity = this.fileConfig.transparent ? 0.9 : 1;

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F10))
            {
                this.fileConfig.showmenu = !this.fileConfig.showmenu;
                this.menuStrip.Visible = this.fileConfig.showmenu;
                this.showMenuToolStripMenuItem.Checked = this.fileConfig.showmenu;


                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void showMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.fileConfig.showmenu = !this.fileConfig.showmenu;
            this.menuStrip.Visible = this.fileConfig.showmenu;
            this.showMenuToolStripMenuItem.Checked = this.fileConfig.showmenu;
        }

        private void transparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.fileConfig.transparent = !this.fileConfig.transparent;
            this.transparentToolStripMenuItem.Checked = this.fileConfig.transparent;
            this.Opacity = this.fileConfig.transparent ? 0.9 : 1;
        }
    }
}