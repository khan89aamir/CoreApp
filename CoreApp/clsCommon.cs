using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CoreApp
{
    /// <summary>
    /// Common class.
    /// </summary>
    public class clsCommon
    {
        /// <summary>
        /// variable for the checking if the for loop is completed or not.
        /// </summary>
        internal static bool IsForCompleted = false;

        /// <summary>
        /// Flag to determine if the thread is running or not.
        /// </summary>
        public static bool IsThreadRunning;

        /// <summary>
        /// Flag to determine whether the DataPopup window is clicked or not.
        /// </summary>
        public static bool IsPoupGridCellClick = false;

        /// <summary>
        /// set the CoreErrorText.
        /// </summary>

        public static string CoreErrorText = "";
        /// <summary>
        /// Filed to set whether you want to see the default error window when exception or error occures
        /// by default the value is True. if you set it to false then you will find the error in CoreErrorText Variable.
        /// </summary>
        public static bool ShowErrorWindow = true;

        /// <summary>
        /// set the isErrorWindowOpen
        /// </summary>
        //internal static bool isErrorWindowOpen = false;
        public static bool isErrorWindowOpen = false;

        /// <summary>
        /// Enum to set the Action for command button status.
        /// </summary>
        public enum ButtonStatus
        {
            /// <summary>
            /// Set this action on Form Load.
            /// </summary>
            Beginning,
            /// <summary>
            /// Set the action when user click on new button.
            /// </summary>
            AfterNew,
            /// <summary>
            /// Set this action when user click on Save button.
            /// </summary>
            AfterSave,
            /// <summary>
            /// Set this action when user click on Edit button.
            /// </summary>
            AfterEdit,
            /// <summary>
            /// Set this action when user click on Update button.
            /// </summary>
            AfterUpdate,
            /// <summary>
            /// Set this action when user click on delete button.
            /// </summary>
            AfterDelete,
            /// <summary>
            /// Set this action when user click on cancel button.
            /// </summary>
            AfterCancel,
            /// <summary>
            /// Set this action when user double click on Grid button.
            /// </summary>
            AfterGridClick
        };

        internal static void ShowError(Exception ex, string strMethod)
        {
            if (ShowErrorWindow)
            {
                ErrorWindow ObjError = new ErrorWindow();
                string strhtml = "<html>" +
                               "<head>" +
                                   "<title></title>" +
                                   "<style type=\"text/css\">" +
                                       ".auto-style1 {" +
                                           "color: #FF0000;" +
                                       "}" +
                                       ".auto-style2 {" +
                                           "color: #0000FF;" +
                                       "}" +
                                   "</style>" +
                               "</head>" +
                               "<body>" +
                                       "<strong><span class=\"auto-style1\">Error Occurred :</span><br />" +
                                       "<span class=\"auto-style2\"><br>Method :</strong></span>" + strMethod +
                                       "<br /><span class=\"auto-style2\"><strong>Exception Text :" +
                               "</strong></span>" + ex.ToString() +
                               "</body>" +
                               "</html>";

                if (ObjError.webBrowser1 != null)
                {
                    if (!clsCommon.isErrorWindowOpen)
                    {
                        ObjError.webBrowser1.DocumentText = strhtml;
                        ObjError.ShowDialog();
                        ObjError.BringToFront();
                    }
                }
                else
                {
                    clsUtility.ShowErrorMessage(Environment.NewLine + "Exception Text : " + ex.ToString() + Environment.NewLine +
                                       "----" + Environment.NewLine + "Note: You are seeing this message in this window because your Thread Apartment State is not STA.", "Error");
                }
            }
            else
            {
                CoreErrorText = ex.ToString();
            }
        }

        internal static void ShowError(string strMessage, string strMethod)
        {
            if (ShowErrorWindow)
            {
                ErrorWindow ObjError = new ErrorWindow();
                ObjError.errstring = "Method name : " + strMethod + Environment.NewLine + "Error :" + strMessage;
                string strhtml = "<html>" +
                               "<head>" +
                                   "<title></title>" +
                                   "<style type=\"text/css\">" +
                                       ".auto-style1 {" +
                                           "color: #FF0000;" +
                                       "}" +
                                       ".auto-style2 {" +
                                           "color: #0000FF;" +
                                       "}" +
                                   "</style>" +
                               "</head>" +
                               "<body>" +
                                       "<strong><span class=\"auto-style1\">Error Occurred :</span><br />" +

                                       "<span class=\"auto-style2\"><br>Method :</strong></span>" + strMethod +

                                       "<br /><span class=\"auto-style2\"><strong>Exception Text :" +
                               "</strong></span>" + strMessage +
                               "</body>" +
                               "</html>";

                if (!clsCommon.isErrorWindowOpen)
                {
                    ObjError.webBrowser1.DocumentText = strhtml;
                    ObjError.ShowDialog();
                    ObjError.BringToFront();
                }
                else
                {
                    clsUtility.ShowErrorMessage(Environment.NewLine + "Exception Text : " + strMessage + Environment.NewLine +
                                         "----" + Environment.NewLine + "Note: You are seeing this message in this window because your Thread Apartment State is not STA.", "Error");
                }
            }
            else
            {
                CoreErrorText = strMessage;
            }
        }
    }
}