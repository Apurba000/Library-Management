CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Salt NVARCHAR(MAX) NOT NULL,
    Role INT NOT NULL DEFAULT 0, -- 0 = Member, 1 = Admin
    IsActive BIT DEFAULT 1,
    LastLoginDate DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Books (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ISBN VARCHAR(13) UNIQUE NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(255) NOT NULL,
    Publisher NVARCHAR(255),
    PublicationYear INT,
    Genre NVARCHAR(100),
    Description NVARCHAR(MAX),
    TotalCopies INT NOT NULL DEFAULT 1,
    AvailableCopies INT NOT NULL DEFAULT 1,
    Location NVARCHAR(100),
    CoverImageUrl NVARCHAR(500),
    CategoryId INT,
    IsActive BIT DEFAULT 1,
    CreatedBy INT,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);

CREATE TABLE Members (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    MemberNumber VARCHAR(20) UNIQUE NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    DateOfBirth DATE,
    MembershipDate DATE DEFAULT GETDATE(),
    MembershipExpiryDate DATE,
    MembershipStatus INT NOT NULL DEFAULT 0, -- 0 = Active, 1 = Suspended, 2 = Expired, 3 = Pending
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE Loans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BookId INT NOT NULL,
    MemberId INT NOT NULL,
    LoanDate DATETIME2 DEFAULT GETDATE(),
    DueDate DATETIME2 NOT NULL,
    ReturnDate DATETIME2 NULL,
    Status INT NOT NULL DEFAULT 0, -- 0 = Borrowed, 1 = Returned, 2 = Overdue
    Notes NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (BookId) REFERENCES Books(Id),
    FOREIGN KEY (MemberId) REFERENCES Members(Id)
);


