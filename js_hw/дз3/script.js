const addForm = document.getElementById("add-form");
const nameInput = document.getElementById("product-name");
const priceInput = document.getElementById("product-price");
const quantityInput = document.getElementById("product-quantity");
const itemsContainer = document.getElementById("cart-items");
const emptyMessage = document.getElementById("empty-message");
const totalValue = document.getElementById("total-value");
const operationLog = document.getElementById("operation-log");
const logStatus = document.getElementById("log-status");
const message = document.getElementById("message");
const moneyFormat = new Intl.NumberFormat("ru-RU", { maximumFractionDigits: 2 });

function addLog(text) {
    const line = document.createElement("li");
    const time = new Date().toLocaleTimeString("ru-RU");
    line.textContent = "[" + time + "] " + text;
    operationLog.prepend(line);
    logStatus.textContent = text;
    console.log("[" + time + "] " + text);
}

const cart = createCart(addLog);
window.cart = cart;

function renderCart() {
    itemsContainer.replaceChildren();
    const items = cart.items;
    emptyMessage.hidden = items.length > 0;

    for (const item of items) {
        const row = document.createElement("div");
        row.className = "product-row";

        const details = document.createElement("div");
        details.className = "product-details";
        const name = document.createElement("strong");
        name.textContent = item.name;
        const price = document.createElement("span");
        price.textContent = moneyFormat.format(item.price) + " монет / шт.";
        details.append(name, price);

        const quantityLabel = document.createElement("label");
        quantityLabel.className = "quantity-label";
        const quantityText = document.createElement("span");
        quantityText.textContent = "Количество";
        const quantity = document.createElement("input");
        quantity.type = "number";
        quantity.min = "1";
        quantity.max = "1000000";
        quantity.step = "1";
        quantity.value = item.quantity;
        quantity.setAttribute("aria-label", "Количество товара «" + item.name + "»");
        quantity.addEventListener("change", function () {
            try {
                cart.updateQuantity(item.id, quantity.valueAsNumber);
                message.textContent = "";
                updateDisplayedTotal();
            } catch (error) {
                message.textContent = error.message;
                quantity.value = item.quantity;
            }
        });
        quantityLabel.append(quantityText, quantity);

        const removeButton = document.createElement("button");
        removeButton.type = "button";
        removeButton.className = "red remove-button";
        removeButton.textContent = "Удалить";
        removeButton.setAttribute("aria-label", "Удалить товар «" + item.name + "»");
        removeButton.addEventListener("click", function () {
            cart.removeItem(item.id);
            message.textContent = "";
            renderCart();
        });

        row.append(details, quantityLabel, removeButton);
        itemsContainer.append(row);
    }

    updateDisplayedTotal();
}

function updateDisplayedTotal() {
    const total = cart.items.reduce((sum, item) => sum + Math.round(item.price * 100) * item.quantity, 0) / 100;
    totalValue.textContent = moneyFormat.format(total);
}

addForm.addEventListener("submit", function (event) {
    event.preventDefault();
    try {
        cart.addItem(nameInput.value, priceInput.valueAsNumber, quantityInput.valueAsNumber);
        addForm.reset();
        message.textContent = "";
        renderCart();
        nameInput.focus();
    } catch (error) {
        message.textContent = error.message;
    }
});

document.getElementById("show-total").addEventListener("click", function () {
    totalValue.textContent = moneyFormat.format(cart.total);
    message.textContent = "";
});

document.getElementById("delete-first").addEventListener("click", function () {
    const first = cart.items[0];
    cart.removeItem(first ? first.id : "первый товар");
    renderCart();
    message.textContent = first ? "" : "Корзина пуста: удалять нечего.";
});

document.getElementById("clear-cart").addEventListener("click", function () {
    cart.clear();
    message.textContent = "";
    renderCart();
});

document.getElementById("clear-log").addEventListener("click", function () {
    operationLog.replaceChildren();
    logStatus.textContent = "Журнал очищен.";
});

renderCart();
