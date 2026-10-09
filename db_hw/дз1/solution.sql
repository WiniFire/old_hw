SELECT DISTINCT u.*
FROM users AS u
JOIN albums AS a ON a.user_id = u.id
WHERE a.rating >= ALL (
    SELECT AVG(a2.rating)
    FROM albums AS a2
    GROUP BY a2.user_id
);

SELECT DISTINCT u.*
FROM users AS u
JOIN albums AS a ON a.user_id = u.id
WHERE a.rating = (
    SELECT MAX(a2.rating)
    FROM albums AS a2
);
