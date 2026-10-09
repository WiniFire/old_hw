const form = document.getElementById("converter-form");
const valueInput = document.getElementById("value-input");
const typeSelect = document.getElementById("type-select");
const resultOutput = document.getElementById("result-output");
const resultType = document.getElementById("result-type");
const resultNote = document.getElementById("result-note");

form.addEventListener("submit", function (event) {
    event.preventDefault();

    const value = valueInput.value;
    let result;

    switch (typeSelect.value) {
        case "Number":
            result = Number(value);
            break;
        case "String":
            result = String(value);
            break;
        case "Boolean":
            result = Boolean(value);
            break;
        default:
            return;
    }

    resultOutput.textContent = typeof result === "string"
        ? JSON.stringify(result)
        : String(result);
    resultType.textContent = "Тип: " + typeof result;
    resultNote.textContent = Number.isNaN(result)
        ? "NaN: введённое значение нельзя преобразовать в число."
        : "";
});
