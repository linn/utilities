using System;
using System.Windows.Forms;
using HardwareProxy;
using Linn.Common.Configuration;

namespace BoardReaderApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.comPort.Text = ConfigurationManager.Configuration["SerialPort"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var boardReader = new BoardReader(this.comPort.Text);
            var hardwareDescriptor = boardReader.Read();

            this.boardId.Text = hardwareDescriptor.BoardId;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.boardId.Text);
        }
    }
}
