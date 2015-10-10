using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HistoryFileTransfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            FileConverter converter = new FileConverter();

            string result = converter.convert();

            MessageBox.Show(result);

        }
    }
}
