CREATE TABLE Products (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Name     TEXT NOT NULL CHECK (length(Name) <= 100),
    Category TEXT NOT NULL CHECK (length(Category) <= 50),
    Quantity INTEGER NOT NULL CHECK (Quantity >= 0),
    Price    NUMERIC NOT NULL CHECK (Price >= 0)
);
