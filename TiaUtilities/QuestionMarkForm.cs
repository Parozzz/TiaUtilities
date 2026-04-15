using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Utility;

namespace TiaUtilities
{
    public partial class QuestionMarkForm : Form
    {
        public QuestionMarkForm()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            this.mainPanel.VisibleChanged += (sender, args) =>
            {
                this.ClientSize = this.mainPanel.Size;
            };

            this.topLabel.Text = this.topLabel.Text.Replace("{v}", Program.VERSION);


            this.softwareGithubLinkLabel.Links[0].Enabled = true;
            this.softwareGithubLinkLabel.Links[0].LinkData = "https://github.com/Parozzz/TiaUtilities";
            this.softwareGithubLinkLabel.LinkClicked += (sender, args) => OpenLink(args.Link);

            this.myReposLinkLabel.Links[0].Enabled = true;
            this.myReposLinkLabel.Links[0].LinkData = "https://github.com/Parozzz";
            this.myReposLinkLabel.LinkClicked += (sender, args) => OpenLink(args.Link);

        }

        private void OpenLink(LinkLabel.Link? link)
        {
            if (link?.LinkData is string url)
            {
                try
                {
                    using Process myProcess = new()
                    {// true is the default, but it is important not to set it to false
                        StartInfo = { UseShellExecute = true, FileName = url }
                    };
                    myProcess.Start();
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            }
        }
    }
}
