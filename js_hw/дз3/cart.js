function createCart(writeLog = console.log) {
    const target = Object.create(null);
    let nextId = 1;

    function checkQuantity(value) {
        if (!Number.isInteger(value) || value < 1 || value > 1000000) {
            throw new RangeError("Количество должно быть целым числом от 1 до 1000000.");
        }
    }

    function makeProduct(id, source) {
        if (!source || typeof source.name !== "string" || !source.name.trim() || source.name.trim().length > 100) {
            throw new TypeError("Введите название товара длиной до 100 символов.");
        }
        if (typeof source.price !== "number" || !Number.isFinite(source.price) || source.price < 0 || source.price > 1000000) {
            throw new RangeError("Цена должна быть числом от 0 до 1000000.");
        }
        checkQuantity(source.quantity);
        let quantity = source.quantity;
        const product = {};

        Object.defineProperties(product, {
            id: { value: id, enumerable: true },
            name: { value: source.name.trim(), enumerable: true },
            price: { value: Math.round((source.price + Number.EPSILON) * 100) / 100, enumerable: true },
            quantity: {
                enumerable: true,
                get() { return quantity; },
                set(value) {
                    try {
                        checkQuantity(value);
                    } catch (error) {
                        writeLog("Изменение количества «" + product.name + "» отклонено: " + error.message);
                        throw error;
                    }
                    const previous = quantity;
                    quantity = value;
                    writeLog("Количество «" + product.name + "»: " + previous + " → " + quantity + ".");
                }
            }
        });

        return product;
    }

    function getItems() {
        return Object.keys(target).map(id => target[id]);
    }

    function calculateTotal() {
        return getItems().reduce((sum, item) => sum + Math.round(item.price * 100) * item.quantity, 0) / 100;
    }

    Object.defineProperties(target, {
        items: { get: getItems },
        total: {
            get() {
                const value = calculateTotal();
                writeLog("Чтение общей стоимости: " + value + " монет.");
                return value;
            }
        },
        addItem: {
            value(name, price, quantity = 1) {
                let id;
                do { id = "item_" + nextId++; } while (Object.hasOwn(target, id));
                cart[id] = { name, price, quantity };
                return cart[id];
            }
        },
        updateQuantity: {
            value(id, quantity) {
                const product = getItems().find(item => item.id === id);
                if (!product) {
                    writeLog("Изменение количества: товар «" + id + "» не найден.");
                    throw new Error("Товар не найден.");
                }
                product.quantity = quantity;
            }
        },
        removeItem: { value(id) { return delete cart[id]; } },
        clear: {
            value() {
                const items = getItems();
                if (items.length === 0) writeLog("Очистка корзины: корзина уже пуста.");
                for (const item of items) delete cart[item.id];
            }
        }
    });

    const cart = new Proxy(target, {
        set(object, property, value) {
            if (typeof property !== "string") throw new TypeError("Некорректный ключ товара.");
            if (Object.hasOwn(object, property)) throw new Error("Свойство уже существует. Для изменения количества используйте quantity.");
            const product = makeProduct(property, value);
            Object.defineProperty(object, property, { value: product, enumerable: true, configurable: true });
            writeLog("Добавлен товар «" + product.name + "»: цена " + product.price + " монет, количество " + product.quantity + ".");
            return true;
        },
        deleteProperty(object, property) {
            const descriptor = Object.getOwnPropertyDescriptor(object, property);
            if (!descriptor) {
                writeLog("Попытка удалить товар «" + String(property) + "»: товар не найден.");
                return true;
            }
            if (!descriptor.configurable) {
                writeLog("Попытка удалить служебное свойство «" + String(property) + "» отклонена.");
                return false;
            }
            writeLog("Попытка удалить товар «" + object[property].name + "».");
            return Reflect.deleteProperty(object, property);
        }
    });

    return cart;
}
