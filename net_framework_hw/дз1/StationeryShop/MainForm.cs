using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StationeryShop
{
    public class MainForm : Form
    {
        private readonly ShopStore store;
        private readonly Dictionary<int, int> cart = new Dictionary<int, int>();
        private readonly DataGridView catalogGrid = MakeGrid();
        private readonly DataGridView cartGrid = MakeGrid();
        private readonly DataGridView ordersGrid = MakeGrid();
        private readonly DataGridView itemsGrid = MakeGrid();
        private readonly TextBox searchBox = new TextBox { Width = 230 };
        private readonly ComboBox categoryBox = new ComboBox { Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown quantityBox = new NumericUpDown { Minimum = 1, Maximum = 1000000, Width = 85 };
        private readonly TextBox customerBox = new TextBox { Width = 220, MaxLength = 100 };
        private readonly Label totalLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), Padding = new Padding(0, 6, 15, 0) };
        private readonly ToolStripStatusLabel status = new ToolStripStatusLabel();

        public MainForm(ShopStore shopStore)
        {
            store = shopStore;
            Text = "КанцМаркет — магазин канцтоваров";
            ClientSize = new Size(1100, 700);
            MinimumSize = new Size(1050, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            root.Controls.Add(new Label { Text = "КанцМаркет", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(30, 75, 105), Dock = DockStyle.Fill, Padding = new Padding(16, 8, 0, 0) }, 0, 0);
            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildCatalog());
            tabs.TabPages.Add(BuildCart());
            tabs.TabPages.Add(BuildOrders());
            root.Controls.Add(tabs, 0, 1);
            var strip = new StatusStrip();
            strip.Items.Add(status);
            root.Controls.Add(strip, 0, 2);
            Controls.Add(root);
            searchBox.TextChanged += (s, e) => RefreshCatalog();
            categoryBox.SelectedIndexChanged += (s, e) => RefreshCatalog();
            ordersGrid.SelectionChanged += (s, e) => ShowOrderItems();
            RefreshAll();
        }

        private TabPage BuildCatalog()
        {
            AddColumn(catalogGrid, "Id", "ID", 60);
            AddColumn(catalogGrid, "Name", "Название", 220);
            AddColumn(catalogGrid, "Category", "Категория", 210);
            AddColumn(catalogGrid, "Price", "Цена, грн", 100, "N2");
            AddColumn(catalogGrid, "Stock", "Остаток", 90);
            var filters = Bar();
            filters.Controls.Add(Caption("Поиск:"));
            filters.Controls.Add(searchBox);
            filters.Controls.Add(Caption("Категория:"));
            filters.Controls.Add(categoryBox);
            var actions = Bar();
            actions.Controls.Add(ActionButton("Добавить", () => EditProduct(null)));
            actions.Controls.Add(ActionButton("Изменить", () => { var p = SelectedProduct(); if (p != null) EditProduct(p); }));
            actions.Controls.Add(ActionButton("Удалить", DeleteProduct));
            actions.Controls.Add(Caption("Количество:"));
            actions.Controls.Add(quantityBox);
            actions.Controls.Add(ActionButton("В корзину", AddToCart));
            return Page("Каталог", filters, catalogGrid, actions);
        }

        private TabPage BuildCart()
        {
            AddColumn(cartGrid, "Id", "ID", 60);
            AddColumn(cartGrid, "Name", "Название", 260);
            AddColumn(cartGrid, "Price", "Цена, грн", 120, "N2");
            AddColumn(cartGrid, "Quantity", "Количество", 100);
            AddColumn(cartGrid, "Total", "Сумма, грн", 130, "N2");
            var top = Bar();
            top.Controls.Add(Caption("Покупатель:"));
            top.Controls.Add(customerBox);
            top.Controls.Add(ActionButton("Убрать позицию", () =>
            {
                if (cartGrid.CurrentRow != null) cart.Remove((int)cartGrid.CurrentRow.Cells[0].Value);
                RefreshCart();
            }));
            top.Controls.Add(ActionButton("Очистить", () => { cart.Clear(); RefreshCart(); }));
            var bottom = Bar();
            bottom.Controls.Add(totalLabel);
            bottom.Controls.Add(ActionButton("Оформить покупку", Checkout));
            return Page("Корзина", top, cartGrid, bottom);
        }

        private TabPage BuildOrders()
        {
            AddColumn(ordersGrid, "Id", "№ заказа", 85);
            AddColumn(ordersGrid, "CreatedAt", "Дата", 170, "dd.MM.yyyy HH:mm");
            AddColumn(ordersGrid, "Customer", "Покупатель", 250);
            AddColumn(ordersGrid, "Total", "Сумма, грн", 130, "N2");
            AddColumn(itemsGrid, "Name", "Товар", 260);
            AddColumn(itemsGrid, "Price", "Цена, грн", 110, "N2");
            AddColumn(itemsGrid, "Quantity", "Количество", 100);
            AddColumn(itemsGrid, "Total", "Сумма, грн", 130, "N2");
            var split = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            split.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            split.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            split.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            split.Controls.Add(ordersGrid, 0, 0);
            split.Controls.Add(new Label { Text = "Состав выбранного заказа", Dock = DockStyle.Fill, Padding = new Padding(4, 6, 0, 0) }, 0, 1);
            split.Controls.Add(itemsGrid, 0, 2);
            var page = new TabPage("Покупки") { Padding = new Padding(12) };
            page.Controls.Add(split);
            return page;
        }

        private void RefreshAll()
        {
            string selected = categoryBox.SelectedItem as string;
            categoryBox.Items.Clear();
            categoryBox.Items.Add("Все категории");
            categoryBox.Items.AddRange(store.GetProducts().Select(p => p.Category).Distinct().OrderBy(c => c).Cast<object>().ToArray());
            categoryBox.SelectedItem = selected;
            if (categoryBox.SelectedIndex < 0) categoryBox.SelectedIndex = 0;
            RefreshCatalog();
            RefreshCart();
            ordersGrid.DataSource = store.GetOrders();
            ShowOrderItems();
            status.Text = "Товаров в каталоге: " + store.GetProducts().Count + "     Покупок: " + store.GetOrders().Count;
        }

        private void RefreshCatalog()
        {
            string search = searchBox.Text.Trim();
            string category = categoryBox.SelectedItem as string;
            catalogGrid.DataSource = store.GetProducts().Where(p =>
                (search.Length == 0 || p.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0) &&
                (categoryBox.SelectedIndex <= 0 || p.Category == category)).ToList();
        }

        private void RefreshCart()
        {
            var products = store.GetProducts();
            var rows = cart.Select(entry =>
            {
                var product = products.First(p => p.Id == entry.Key);
                return new { Id = product.Id, Name = product.Name, Price = product.Price, Quantity = entry.Value, Total = product.Price * entry.Value };
            }).ToList();
            cartGrid.DataSource = rows;
            totalLabel.Text = "Итого: " + rows.Sum(r => r.Total).ToString("N2") + " грн";
        }

        private Product SelectedProduct()
        {
            var product = catalogGrid.CurrentRow == null ? null : catalogGrid.CurrentRow.DataBoundItem as Product;
            if (product == null) MessageBox.Show(this, "Выберите товар в каталоге.", "Каталог");
            return product;
        }

        private void EditProduct(Product product)
        {
            using (var dialog = new ProductForm(product))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                store.SaveProduct(dialog.Product);
                RefreshAll();
            }
        }

        private void DeleteProduct()
        {
            var product = SelectedProduct();
            if (product == null) return;
            if (MessageBox.Show(this, "Удалить «" + product.Name + "»?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            store.DeleteProduct(product.Id);
            cart.Remove(product.Id);
            RefreshAll();
        }

        private void AddToCart()
        {
            var product = SelectedProduct();
            if (product == null) return;
            int existing;
            cart.TryGetValue(product.Id, out existing);
            int count = existing + (int)quantityBox.Value;
            if (count > product.Stock)
                throw new InvalidOperationException("На складе только " + product.Stock + " шт. Учитывается количество в корзине.");
            cart[product.Id] = count;
            RefreshCart();
            status.Text = "Добавлено в корзину: " + product.Name;
        }

        private void Checkout()
        {
            if (cart.Count == 0) throw new InvalidOperationException("Корзина пуста.");
            if (string.IsNullOrWhiteSpace(customerBox.Text)) throw new InvalidOperationException("Введите имя покупателя.");
            if (MessageBox.Show(this, "Оформить покупку?\n" + totalLabel.Text, "Покупка", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var order = store.Checkout(customerBox.Text, cart);
            cart.Clear();
            customerBox.Clear();
            RefreshAll();
            MessageBox.Show(this, "Покупка №" + order.Id + " оформлена.\nСумма: " + order.Total.ToString("N2") + " грн", "Покупка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowOrderItems()
        {
            var order = ordersGrid.CurrentRow == null ? null : ordersGrid.CurrentRow.DataBoundItem as Order;
            itemsGrid.DataSource = order == null ? new List<OrderItem>() : order.Items;
        }

        private Button ActionButton(string text, Action action)
        {
            var button = new Button { Text = text, AutoSize = true, Height = 34, Padding = new Padding(7, 3, 7, 3) };
            button.Click += (s, e) =>
            {
                try { action(); }
                catch (Exception error) { MessageBox.Show(this, error.Message, "Операция не выполнена", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };
            return button;
        }

        private static DataGridView MakeGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill, AutoGenerateColumns = false, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                RowHeadersVisible = false, BackgroundColor = Color.White,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True, Padding = new Padding(4) }
            };
        }

        private static void AddColumn(DataGridView grid, string property, string title, int width, string format = "")
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = title, FillWeight = width, DefaultCellStyle = new DataGridViewCellStyle { Format = format } });
        }

        private static FlowLayoutPanel Bar()
        {
            return new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0, 6, 0, 0), WrapContents = false };
        }

        private static Label Caption(string text)
        {
            return new Label { Text = text, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
        }

        private static TabPage Page(string title, Control top, Control center, Control bottom)
        {
            var page = new TabPage(title) { Padding = new Padding(12) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            layout.Controls.Add(top, 0, 0);
            layout.Controls.Add(center, 0, 1);
            layout.Controls.Add(bottom, 0, 2);
            page.Controls.Add(layout);
            return page;
        }
    }
}
