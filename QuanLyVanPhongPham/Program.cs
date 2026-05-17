using System;
using System.Windows.Forms;

namespace QuanLyVanPhongPham
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainAppContext());
        }
    }
    public class MainAppContext : ApplicationContext
    {
        public MainAppContext()
        {
            AppContext.NavTo(null, new QuanLySanPham());
        }
    }
    public class AppContext
    {
        public static void NavTo(Form current, Form next)
        {
            next.Show();
            current?.Hide();
        }
    }
}