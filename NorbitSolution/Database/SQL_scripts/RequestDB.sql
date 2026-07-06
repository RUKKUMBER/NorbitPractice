-- Выборка данных с фильтрацией, сортировкой
SELECT Title, Price, ReleaseDate
FROM Games
WHERE Price < 30
ORDER BY Price ASC;

-- Изменение
UPDATE Games
SET Price = Price * 1.10
FROM Games g
    INNER JOIN Publishers p ON g.PublisherId = p.PublisherId
WHERE p.PublisherName = 'Valve';

-- Удаление
DELETE FROM Reviews
WHERE LEN(ReviewText) < 10;

-- Выборка с группировкой
SELECT p.PublisherName, COUNT(g.GameId) AS GameCount
FROM Publishers p
    INNER JOIN Games g ON p.PublisherId = g.PublisherId
GROUP BY p.PublisherName
HAVING COUNT(g.GameId) > 1
ORDER BY GameCount DESC;

-- Пересечение
SELECT g.Title,
       STRING_AGG(gen.GenreName, ', ') AS Genres,
       STRING_AGG(plat.PlatformName, ', ') AS Platforms
FROM Games g
    INNER JOIN GameGenres gg ON g.GameId = gg.GameId
    INNER JOIN Genres gen ON gg.GenreId = gen.GenreId
    INNER JOIN GamePlatforms gp ON g.GameId = gp.GameId
    INNER JOIN Platforms plat ON gp.PlatformId = plat.PlatformId
GROUP BY g.Title
ORDER BY g.Title;

-- LEFT JOIN
SELECT g.Title,
       r.ReviewText,
       r.Rating
FROM Games g
    LEFT JOIN Reviews r ON g.GameId = r.GameId
ORDER BY g.Title, r.Rating DESC;

-- RIGHT JOIN
SELECT plat.PlatformName,
       g.Title AS GameTitle
FROM GamePlatforms gp
    RIGHT JOIN Platforms plat ON gp.PlatformId = plat.PlatformId
    LEFT JOIN Games g ON gp.GameId = g.GameId
ORDER BY plat.PlatformName, g.Title;

-- FULL OUTER JOIN
SELECT g.Title AS Game,
       plat.PlatformName AS Platform
FROM Games g
    FULL OUTER JOIN GamePlatforms gp ON g.GameId = gp.GameId
    FULL OUTER JOIN Platforms plat ON gp.PlatformId = plat.PlatformId
ORDER BY Game, Platform;
GO