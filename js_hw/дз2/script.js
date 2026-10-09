const textInput = document.getElementById("text-input");
const decoratorList = document.getElementById("decorator-list");
const resultOutput = document.getElementById("result-output");
const inputCount = document.getElementById("input-count");
const resultCount = document.getElementById("result-count");
const activeCount = document.getElementById("active-count");
const pipeline = document.getElementById("pipeline");
const pipelineNote = document.getElementById("pipeline-note");
const copyStatus = document.getElementById("copy-status");
const checkboxes = [];
let currentResult = "";

for (const definition of decoratorDefinitions) {
    const card = document.createElement("label");
    card.className = "decorator-card";

    const checkbox = document.createElement("input");
    checkbox.type = "checkbox";
    checkbox.value = definition.id;
    checkbox.id = "decorator-" + definition.id;
    checkbox.addEventListener("change", updateResult);
    checkboxes.push(checkbox);

    const description = document.createElement("span");
    description.className = "decorator-description";

    const title = document.createElement("strong");
    title.textContent = definition.title;
    const detail = document.createElement("span");
    detail.className = "small";
    detail.textContent = definition.description;

    description.append(title, detail);
    card.append(checkbox, description);
    decoratorList.append(card);
}

function updateResult() {
    const activeIds = checkboxes.filter(checkbox => checkbox.checked).map(checkbox => checkbox.value);
    const activeDefinitions = decoratorDefinitions.filter(definition => activeIds.includes(definition.id));

    currentResult = applyDecorators(textInput.value, activeIds);
    resultOutput.textContent = currentResult;
    resultOutput.classList.toggle("empty", currentResult.length === 0);
    inputCount.textContent = "Символов: " + countCharacters(textInput.value);
    resultCount.textContent = "Символов в результате: " + countCharacters(currentResult);
    activeCount.textContent = activeIds.length + " / " + decoratorDefinitions.length;
    copyStatus.textContent = "";

    for (const checkbox of checkboxes) {
        checkbox.closest("label").classList.toggle("active", checkbox.checked);
    }

    pipeline.replaceChildren();
    const source = document.createElement("span");
    source.className = "pipeline-source";
    source.textContent = "Исходный текст";
    pipeline.append(source);

    for (const definition of activeDefinitions) {
        const arrow = document.createElement("span");
        arrow.className = "arrow";
        arrow.textContent = "→";
        arrow.setAttribute("aria-hidden", "true");
        const badge = document.createElement("span");
        badge.className = "badge";
        badge.textContent = definition.title;
        pipeline.append(arrow, badge);
    }

    pipelineNote.textContent = activeIds.length === 0
        ? "Декораторы не выбраны. Результат совпадает с исходным текстом."
        : "Каждый декоратор оборачивает предыдущую функцию и преобразует её результат.";

    if (activeIds.includes("uppercase") && activeIds.includes("lowercase")) {
        pipelineNote.textContent += " Нижний регистр применяется после верхнего, поэтому буквы будут строчными.";
    }
}

textInput.addEventListener("input", updateResult);

document.getElementById("reset-button").addEventListener("click", function () {
    for (const checkbox of checkboxes) checkbox.checked = false;
    updateResult();
});

document.getElementById("copy-button").addEventListener("click", async function () {
    const textToCopy = currentResult;
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(textToCopy);
        } else {
            const temporaryInput = document.createElement("textarea");
            temporaryInput.value = textToCopy;
            temporaryInput.style.position = "fixed";
            temporaryInput.style.left = "-9999px";
            document.body.append(temporaryInput);
            const focusedElement = document.activeElement;
            let copied;
            try {
                temporaryInput.select();
                copied = document.execCommand("copy");
            } finally {
                temporaryInput.remove();
                if (focusedElement) focusedElement.focus();
            }
            if (!copied) throw new Error("Copy failed");
        }
        if (currentResult === textToCopy) copyStatus.textContent = "Скопировано";
    } catch (error) {
        copyStatus.textContent = "Выделите результат и скопируйте вручную.";
    }
});

updateResult();
