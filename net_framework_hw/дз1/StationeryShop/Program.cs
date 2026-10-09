using System;
using System.IO;
using System.Windows.Forms;

namespace StationeryShop
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StationeryShop");
                var store = new ShopStore(Path.Combine(folder, "shop.xml"));
                Application.Run(new MainForm(store));
            }
            catch (Exception error)
            {
                MessageBox.Show("Не удалось открыть магазин.\n" + error.Message,
                    "КанцМаркет", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
