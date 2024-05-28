using FrontEnd.Utils;
using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends <see cref="MenuItem"/> and it is meant to open a <see cref="Curtain"/> object.
    /// For example:
    /// <code>
    /// &lt;Menu VerticalAlignment="Top">
    ///     &lt;fr:OpenCurtain Click="Open"/>
    ///     ...
    /// &lt;/Menu>
    /// </code>
    /// </summary>
    public class OpenCurtain : MenuItem
    {
        public OpenCurtain() 
        {
            Header = Helper.LoadFromStrings("openCurtain");
            VerticalAlignment = VerticalAlignment.Center;
            ToolTip = "Open Curtain";
        }
    }
}
