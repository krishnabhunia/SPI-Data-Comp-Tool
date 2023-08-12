using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();

            try
            {
                this.Text = String.Format("About {0}", AssemblyTitle);
                this.labelProductName.Text = AssemblyProduct;
                this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
                this.labelCopyright.Text = AssemblyCopyright;
                this.labelCompanyName.Text = AssemblyCompany;
                this.textBoxDescription.Text = AssemblyDescription;
                //versionNumber = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

            }
            catch (Exception ex)
            {
                textBoxDescription.Text = ex.Message.ToString();
                if (GlobalDebug.boolIsGlobalDebug)
                {
                    textBoxDescription.Text = ex.ToString();
                }
            }

        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                try
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title != "")
                        {
                            return titleAttribute.Title;
                        }
                    }
                    return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
                catch (Exception ex)
                {
                    textBoxDescription.Text = ex.Message.ToString();
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        textBoxDescription.Text = ex.ToString();
                    }
                    return ex.Message.ToString();
                }
            }
        }

        public string AssemblyVersion
        {
            get
            {
                try
                {
                    if (Debugger.IsAttached)
                    {
                        return Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    }
                    else
                    {
                        return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

                    }
                }
                catch (Exception ex)
                {
                    textBoxDescription.Text = ex.Message.ToString();
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        textBoxDescription.Text = ex.ToString();
                    }
                    return ex.Message.ToString();
                }
            }
        }

        public string AssemblyDescription
        {
            get
            {
                try
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
                catch (Exception ex)
                {
                    textBoxDescription.Text = ex.Message.ToString();
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        textBoxDescription.Text = ex.ToString();
                    }
                    return ex.Message.ToString();
                }
            }
        }

        public string AssemblyProduct
        {
            get
            {
                try
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyProductAttribute)attributes[0]).Product;
                }
                catch (Exception ex)
                {
                    textBoxDescription.Text = ex.Message.ToString();
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        textBoxDescription.Text = ex.ToString();
                    }
                    return ex.Message.ToString();
                }

            }
        }

        public string AssemblyCopyright
        {
            get
            {
                try
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
                catch (Exception ex)
                {
                    textBoxDescription.Text = ex.Message.ToString();
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        textBoxDescription.Text = ex.ToString();
                    }
                    return ex.Message.ToString();
                }
            }
        }

        public string AssemblyCompany
        {
            get
            {
                try
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
                }
                catch (Exception ex)
                {
                    textBoxDescription.Text = ex.Message.ToString();
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        textBoxDescription.Text = ex.ToString();
                    }
                    return ex.Message.ToString();
                }
            }
        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                textBoxDescription.Text = ex.Message.ToString();
                if (GlobalDebug.boolIsGlobalDebug)
                {
                    textBoxDescription.Text = ex.ToString();
                }
            }
        }

        private void AboutBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                textBoxDescription.Text = ex.Message.ToString();
                if (GlobalDebug.boolIsGlobalDebug)
                {
                    textBoxDescription.Text = ex.ToString();
                }
            }

        }
    }
}
