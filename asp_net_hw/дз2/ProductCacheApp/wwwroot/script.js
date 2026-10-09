const output = document.getElementById("product-output");
const info = document.getElementById("cache-info");
const message = document.getElementById("message");
const log = document.getElementById("log");
const buttons = Array.from(document.querySelectorAll("button"));
let cache = null;
let busy = false;

function addLog(text) {
    const item = document.createElement("li");
    item.textContent = new Date().toLocaleTimeString("ru-RU") + " — " + text;
    log.prepend(item);
}

function showCache() {
    if (!cache) return;
    output.textContent = JSON.stringify(cache.product, null, 2);
    const remaining = Math.max(0, Math.ceil((cache.expiresAt - Date.now()) / 1000));
    info.textContent = "ETag: " + cache.etag + " · " + (remaining > 0 ? "Кэш свежий ещё " + remaining + " сек." : "Кэш устарел.");
}

function expiration(response) {
    const header = response.headers.get("Cache-Control") || "";
    const match = header.match(/(?:^|,)\s*max-age=(\d+)/i);
    return Date.now() + (match ? Number(match[1]) : 0) * 1000;
}

async function getProduct(forceValidation = false) {
    if (cache && !forceValidation && Date.now() < cache.expiresAt) {
        addLog("200 OK из клиентского кэша. Запрос на сервер не отправлялся.");
        showCache();
        return;
    }

    const headers = {};
    if (cache) headers["If-None-Match"] = cache.etag;
    addLog("GET /api/products/123" + (cache ? " · If-None-Match: " + cache.etag : " · кэш пуст"));

    const response = await fetch("/api/products/123", { headers, cache: "no-store" });
    if (response.status === 304 && cache) {
        cache.expiresAt = expiration(response);
        cache.etag = response.headers.get("ETag") || cache.etag;
        addLog("304 Not Modified · ETag: " + cache.etag + ". Используется сохранённый товар.");
    } else {
        if (!response.ok) throw new Error("Ошибка запроса: HTTP " + response.status);
        const etag = response.headers.get("ETag");
        if (!etag) throw new Error("Сервер не передал ETag.");
        cache = { product: await response.json(), etag, expiresAt: expiration(response) };
        addLog("200 OK · ETag: " + cache.etag + " · " + response.headers.get("Cache-Control") + ". Кэш обновлён.");
    }
    showCache();
}

async function run(action) {
    if (busy) return;
    busy = true;
    buttons.forEach(button => { button.disabled = true; });
    message.textContent = "";
    try {
        await action();
    } catch (error) {
        message.textContent = error.message;
        addLog(error.message);
    } finally {
        busy = false;
        buttons.forEach(button => { button.disabled = false; });
    }
}

document.getElementById("get-product").addEventListener("click", () => run(() => getProduct()));
document.getElementById("expire-cache").addEventListener("click", () => run(() => {
    if (cache) cache.expiresAt = 0;
    return getProduct(true);
}));

document.getElementById("update-form").addEventListener("submit", event => {
    event.preventDefault();
    run(async () => {
        const name = document.getElementById("name").value.trim();
        const price = document.getElementById("price").valueAsNumber;
        if (!name || !Number.isFinite(price)) throw new Error("Заполните название и цену.");
        const response = await fetch("/api/products/123", {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            cache: "no-store",
            body: JSON.stringify({ name, price })
        });
        if (!response.ok) throw new Error("Товар не сохранён: HTTP " + response.status);
        addLog("PUT /api/products/123 → 200 OK · ETag сервера: " + response.headers.get("ETag") + ". Клиентский кэш ещё содержит прежний ответ.");
        message.textContent = "Товар сохранён. Проверьте устаревший кэш для получения актуального ответа.";
    });
});

setInterval(showCache, 1000);
