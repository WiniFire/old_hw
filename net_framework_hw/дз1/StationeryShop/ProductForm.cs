using System;
using System.Drawing;
using System.Windows.Forms;

namespace StationeryShop
{
    public class ProductForm : Form
    {
        private readonly TextBox nameBox = new TextBox { MaxLength = 100, Dock = DockStyle.Fill };
        private readonly TextBox categoryBox = new TextBox { MaxLength = 60, Dock = DockStyle.Fill };
        private readonly NumericUpDown priceBox = new NumericUpDown { Minimum = 0.01m, Maximum = 1000000m, DecimalPlaces = 2, Dock = DockStyle.Fill };
        private readonly NumericUpDown stockBox = new NumericUpDown { Minimum = 0, Maximum = 1000000, Dock = DockStyle.Fill };
        private readonly int id;
        public Product Product { get; private set; }

        public ProductForm(Product product)
        {
            id = product == null ? 0 : product.Id;
            Text = product == null ? "Новый товар" : "Изменение товара";
            Font = new Font("Segoe UI", 10);
            ClientSize = new Size(460, 280);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 5 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            AddField(layout, "Название", nameBox, 0);
            AddField(layout, "Категория", categoryBox, 1);
            AddField(layout, "Цена, грн", priceBox, 2);
            AddField(layout, "Остаток, шт.", stockBox, 3);
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var cancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, AutoSize = true };
            var save = new Button { Text = "Сохранить", AutoSize = true };
            save.Click += SaveClick;
            buttons.Controls.Add(cancel);
            buttons.Controls.Add(save);
            layout.Controls.Add(buttons, 0, 4);
            layout.SetColumnSpan(buttons, 2);
            Controls.Add(layout);
            AcceptButton = save;
            CancelButton = cancel;
            if (product != null)
            {
                nameBox.Text = product.Name;
                categoryBox.Text = product.Category;
                priceBox.Value = product.Price;
                stockBox.Value = product.Stock;
            }
        }

        private static void AddField(TableLayoutPanel layout, string text, Control input, int row)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.Controls.Add(new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
            layout.Controls.Add(input, 1, row);
        }

        private void SaveClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text) || string.IsNullOrWhiteSpace(categoryBox.Text))
            {
                MessageBox.Show(this, "Заполните название и категорию.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Product = new Product { Id = id, Name = nameBox.Text.Trim(), Category = categoryBox.Text.Trim(), Price = priceBox.Value, Stock = (int)stockBox.Value };
            DialogResult = DialogResult.OK;
        }
    }
}
