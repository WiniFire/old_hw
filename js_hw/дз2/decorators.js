function createDecorator(transform) {
    return function (originalFunction) {
        return function (text) {
            return transform(originalFunction(text));
        };
    };
}

function countCharacters(text) {
    return Array.from(text).length;
}

const uppercaseDecorator = createDecorator(text => text.toUpperCase());
const lowercaseDecorator = createDecorator(text => text.toLowerCase());
const trimDecorator = createDecorator(text => text.trim());
const reverseDecorator = createDecorator(text => Array.from(text).reverse().join(""));
const underscoreDecorator = createDecorator(text => text.replace(/ /g, "_"));
const quoteDecorator = createDecorator(text => "«" + text + "»");
const characterCountDecorator = createDecorator(text => text + "\n[Символов: " + countCharacters(text) + "]");

const decoratorDefinitions = [
    { id: "uppercase", title: "Верхний регистр", description: "привет → ПРИВЕТ", decorate: uppercaseDecorator },
    { id: "lowercase", title: "Нижний регистр", description: "ПРИВЕТ → привет", decorate: lowercaseDecorator },
    { id: "trim", title: "Убрать пробелы по краям", description: "Удаляет начальные и конечные пробельные символы", decorate: trimDecorator },
    { id: "reverse", title: "Развернуть текст", description: "Привет → тевирП", decorate: reverseDecorator },
    { id: "underscore", title: "Пробелы в подчёркивания", description: "два слова → два_слова", decorate: underscoreDecorator },
    { id: "quotes", title: "Добавить кавычки", description: "текст → «текст»", decorate: quoteDecorator },
    { id: "count", title: "Подсчитать символы", description: "Добавляет количество символов после преобразований", decorate: characterCountDecorator }
];

function applyDecorators(text, activeIds) {
    let decoratedFunction = value => value;

    for (const definition of decoratorDefinitions) {
        if (activeIds.includes(definition.id)) {
            decoratedFunction = definition.decorate(decoratedFunction);
        }
    }

    return decoratedFunction(text);
}
