-- Create Payments table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments')
BEGIN
    CREATE TABLE Payments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TransactionRef NVARCHAR(100) NOT NULL UNIQUE,
        SenderAccount NVARCHAR(50) NOT NULL,
        ReceiverAccount NVARCHAR(50) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Currency NVARCHAR(3) NOT NULL DEFAULT 'KES',
        Status NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        Description NVARCHAR(500) NULL,
        
        INDEX IX_Payments_TransactionRef (TransactionRef),
        INDEX IX_Payments_CreatedAt (CreatedAt DESC)
    );
END
GO
