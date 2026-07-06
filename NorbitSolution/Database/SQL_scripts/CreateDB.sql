CREATE DATABASE ComputerGames;
GO

USE ComputerGames;
GO

CREATE TABLE Publishers (
    PublisherId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PublisherName NVARCHAR(200) NOT NULL,
    Country NVARCHAR(100) NULL
);
CREATE UNIQUE INDEX UQ_Publishers_Name ON Publishers(PublisherName);

CREATE TABLE Platforms (
    PlatformId INT IDENTITY PRIMARY KEY,
    PlatformName NVARCHAR(100) NOT NULL,
    Manufacturer NVARCHAR(100) NULL,
    ReleaseDate DATETIME NULL
);

CREATE TABLE Genres (
    GenreId INT IDENTITY PRIMARY KEY,
    GenreName NVARCHAR(100) NOT NULL
);

CREATE TABLE Games (
    GameId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(300) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Price DECIMAL(10,2) NOT NULL DEFAULT 0,
    ReleaseDate DATETIME NULL,
    HasMultiplayer BIT NOT NULL DEFAULT 0,
    PublisherId UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_Games_Publishers FOREIGN KEY (PublisherId)
        REFERENCES Publishers(PublisherId) ON DELETE SET NULL
);

CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NULL
);

CREATE TABLE Reviews (
    ReviewId INT IDENTITY PRIMARY KEY,
    GameId UNIQUEIDENTIFIER NOT NULL,
    UserId INT NOT NULL,
    ReviewText NVARCHAR(MAX) NULL,
    Rating DECIMAL(2,1) NOT NULL DEFAULT 0.0,
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Games FOREIGN KEY (GameId)
        REFERENCES Games(GameId) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT UQ_Reviews_GameUser UNIQUE (GameId, UserId)
);

CREATE TABLE GameGenres (
    GameId UNIQUEIDENTIFIER NOT NULL,
    GenreId INT NOT NULL,
    CONSTRAINT PK_GameGenres PRIMARY KEY (GameId, GenreId),
    CONSTRAINT FK_GameGenres_Games FOREIGN KEY (GameId)
        REFERENCES Games(GameId) ON DELETE CASCADE,
    CONSTRAINT FK_GameGenres_Genres FOREIGN KEY (GenreId)
        REFERENCES Genres(GenreId) ON DELETE CASCADE
);

CREATE TABLE GamePlatforms (
    GameId UNIQUEIDENTIFIER NOT NULL,
    PlatformId INT NOT NULL,
    CONSTRAINT PK_GamePlatforms PRIMARY KEY (GameId, PlatformId),
    CONSTRAINT FK_GamePlatforms_Games FOREIGN KEY (GameId)
        REFERENCES Games(GameId) ON DELETE CASCADE,
    CONSTRAINT FK_GamePlatforms_Platforms FOREIGN KEY (PlatformId)
        REFERENCES Platforms(PlatformId) ON DELETE CASCADE
);
GO

INSERT INTO Publishers (PublisherName, Country)
VALUES 
('Valve', 'USA'),
('CD Projekt Red', 'Poland'),
('Nintendo', 'Japan');

INSERT INTO Platforms (PlatformName, Manufacturer)
VALUES 
('PC', 'Various'),
('PlayStation 5', 'Sony'),
('Nintendo Switch', 'Nintendo');

INSERT INTO Genres (GenreName)
VALUES 
('Action'),
('RPG'),
('Adventure'),
('Shooter');

INSERT INTO Games (Title, Description, Price, ReleaseDate, HasMultiplayer, PublisherId)
SELECT 
    src.Title,
    src.Description,
    src.Price,
    src.ReleaseDate,
    src.HasMultiplayer,
    p.PublisherId
FROM 
    (VALUES
        ('Half-Life 2', 'A classic first-person shooter', 9.99, '2004-11-16', 0, 'Valve'),
        ('The Witcher 3: Wild Hunt', 'Open-world RPG', 29.99, '2015-05-19', 0, 'CD Projekt Red'),
        ('Portal 2', 'Puzzle-platformer with co-op', 14.99, '2011-04-19', 1, 'Valve'),
        ('Cyberpunk 2077', 'Futuristic open-world RPG', 59.99, '2020-12-10', 0, 'CD Projekt Red'),
        ('Super Mario Odyssey', '3D platformer', 49.99, '2017-10-27', 0, 'Nintendo')
    ) AS src(Title, Description, Price, ReleaseDate, HasMultiplayer, PublisherName)
    JOIN Publishers p ON p.PublisherName = src.PublisherName;

INSERT INTO Users (UserName, Email)
VALUES 
('GamerOne', 'one@example.com'),
('GamerTwo', 'two@example.com');

INSERT INTO Reviews (GameId, UserId, ReviewText, Rating, ReviewDate)
SELECT 
    g.GameId,
    u.UserId,
    src.ReviewText,
    src.Rating,
    GETDATE()
FROM 
    (VALUES
        ('Half-Life 2', 'GamerOne', 'Masterpiece!', 9.5),
        ('The Witcher 3: Wild Hunt', 'GamerOne', 'Best RPG ever', 9.8),
        ('Portal 2', 'GamerTwo', 'Great co-op puzzles', 9.0),
        ('Cyberpunk 2077', 'GamerTwo', 'Had bugs but improved', 7.5)
    ) AS src(GameTitle, UserName, ReviewText, Rating)
    JOIN Games g ON g.Title = src.GameTitle
    JOIN Users u ON u.UserName = src.UserName;

INSERT INTO GameGenres (GameId, GenreId)
SELECT 
    g.GameId,
    gen.GenreId
FROM 
    (VALUES
        ('Half-Life 2', 'Shooter'),
        ('Half-Life 2', 'Action'),
        ('The Witcher 3: Wild Hunt', 'RPG'),
        ('The Witcher 3: Wild Hunt', 'Action'),
        ('Portal 2', 'Adventure'),
        ('Portal 2', 'Action'),
        ('Cyberpunk 2077', 'RPG'),
        ('Cyberpunk 2077', 'Action'),
        ('Super Mario Odyssey', 'Adventure'),
        ('Super Mario Odyssey', 'Action')
    ) AS src(GameTitle, GenreName)
    JOIN Games g ON g.Title = src.GameTitle
    JOIN Genres gen ON gen.GenreName = src.GenreName;

INSERT INTO GamePlatforms (GameId, PlatformId)
SELECT 
    g.GameId,
    p.PlatformId
FROM 
    (VALUES
        ('Half-Life 2', 'PC'),
        ('The Witcher 3: Wild Hunt', 'PC'),
        ('The Witcher 3: Wild Hunt', 'PlayStation 5'),
        ('Portal 2', 'PC'),
        ('Cyberpunk 2077', 'PC'),
        ('Cyberpunk 2077', 'PlayStation 5'),
        ('Super Mario Odyssey', 'Nintendo Switch')
    ) AS src(GameTitle, PlatformName)
    JOIN Games g ON g.Title = src.GameTitle
    JOIN Platforms p ON p.PlatformName = src.PlatformName;
GO