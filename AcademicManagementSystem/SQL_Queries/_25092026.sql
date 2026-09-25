select * from Student

;WITH Numbers AS
(
    SELECT TOP (67)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNo
    FROM sys.all_objects
),
RandomStudents AS
(
    SELECT
        RowNo,

        -- Gender: 1 = Male, 2 = Female
        CASE 
            WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN 1
            ELSE 2
        END AS Gender,

        ABS(CHECKSUM(NEWID())) AS Rand1,
        ABS(CHECKSUM(NEWID())) AS Rand2,
        ABS(CHECKSUM(NEWID())) AS Rand3,
        ABS(CHECKSUM(NEWID())) AS Rand4
    FROM Numbers
),
StudentData AS
(
    SELECT
        RowNo,
        Gender,

        CASE 
            WHEN Gender = 1 THEN
                CASE Rand1 % 20
                    WHEN 0 THEN 'Muhammad Ahmed'
                    WHEN 1 THEN 'Ali Raza'
                    WHEN 2 THEN 'Hassan Khan'
                    WHEN 3 THEN 'Usman Ahmed'
                    WHEN 4 THEN 'Hamza Ali'
                    WHEN 5 THEN 'Abdul Rehman'
                    WHEN 6 THEN 'Bilal Ahmed'
                    WHEN 7 THEN 'Saad Khan'
                    WHEN 8 THEN 'Talha Siddiqui'
                    WHEN 9 THEN 'Zain Ahmed'
                    WHEN 10 THEN 'Ahsan Ali'
                    WHEN 11 THEN 'Fahad Khan'
                    WHEN 12 THEN 'Owais Ahmed'
                    WHEN 13 THEN 'Huzaifa Ali'
                    WHEN 14 THEN 'Shayan Khan'
                    WHEN 15 THEN 'Farhan Ahmed'
                    WHEN 16 THEN 'Taha Siddiqui'
                    WHEN 17 THEN 'Danish Ali'
                    WHEN 18 THEN 'Salman Khan'
                    ELSE 'Adnan Ahmed'
                END
            ELSE
                CASE Rand1 % 20
                    WHEN 0 THEN 'Ayesha Khan'
                    WHEN 1 THEN 'Fatima Ahmed'
                    WHEN 2 THEN 'Maryam Ali'
                    WHEN 3 THEN 'Zainab Siddiqui'
                    WHEN 4 THEN 'Hira Khan'
                    WHEN 5 THEN 'Mahnoor Ahmed'
                    WHEN 6 THEN 'Iqra Ali'
                    WHEN 7 THEN 'Anum Khan'
                    WHEN 8 THEN 'Laiba Ahmed'
                    WHEN 9 THEN 'Sana Ali'
                    WHEN 10 THEN 'Mehwish Khan'
                    WHEN 11 THEN 'Amna Ahmed'
                    WHEN 12 THEN 'Rabia Ali'
                    WHEN 13 THEN 'Kinza Khan'
                    WHEN 14 THEN 'Komal Ahmed'
                    WHEN 15 THEN 'Maham Ali'
                    WHEN 16 THEN 'Eman Khan'
                    WHEN 17 THEN 'Nimra Ahmed'
                    WHEN 18 THEN 'Hafsa Ali'
                    ELSE 'Rimsha Khan'
                END
        END AS FullName,

        Rand2,
        Rand3,
        Rand4
    FROM RandomStudents
)
INSERT INTO Student
(
    FullName,
    Email,
    Phone,
    Gender,
    DateOfBirth,
    Address,
    AdmissionDate,
    IsActive
)
SELECT
    FullName,

    LOWER(
        REPLACE(FullName, ' ', '.')
        + CAST(RowNo AS VARCHAR(10))
        + '@gmail.com'
    ) AS Email,

    '03'
    + RIGHT('00' + CAST(ABS(Rand2) % 100 AS VARCHAR(2)), 2)
    + RIGHT('0000000' + CAST(ABS(Rand3) % 10000000 AS VARCHAR(7)), 7)
        AS Phone,

    Gender,

    -- Random age roughly 17-35 years
    DATEADD
    (
        DAY,
        -(6200 + (ABS(Rand3) % 6500)),
        GETDATE()
    ) AS DateOfBirth,

    CASE ABS(Rand4) % 15
        WHEN 0 THEN 'North Nazimabad, Karachi'
        WHEN 1 THEN 'Gulshan-e-Iqbal, Karachi'
        WHEN 2 THEN 'Federal B Area, Karachi'
        WHEN 3 THEN 'New Karachi, Karachi'
        WHEN 4 THEN 'Nazimabad, Karachi'
        WHEN 5 THEN 'Buffer Zone, Karachi'
        WHEN 6 THEN 'North Karachi, Karachi'
        WHEN 7 THEN 'Gulistan-e-Johar, Karachi'
        WHEN 8 THEN 'Korangi, Karachi'
        WHEN 9 THEN 'Malir, Karachi'
        WHEN 10 THEN 'Shah Faisal Colony, Karachi'
        WHEN 11 THEN 'PECHS, Karachi'
        WHEN 12 THEN 'Saddar, Karachi'
        WHEN 13 THEN 'Landhi, Karachi'
        ELSE 'Scheme 33, Karachi'
    END AS Address,

    -- Random admission during roughly the last 2 years
    DATEADD
    (
        DAY,
        -(ABS(Rand2) % 730),
        GETDATE()
    ) AS AdmissionDate,

    -- Around 90% active
    CASE
        WHEN ABS(Rand4) % 10 = 0 THEN 0
        ELSE 1
    END AS IsActive

FROM StudentData;


;WITH Numbers AS
(
    SELECT TOP (26)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNo
    FROM sys.all_objects
),
RandomStudents AS
(
    SELECT
        RowNo,
        CASE 
            WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN 1
            ELSE 2
        END AS Gender,

        ABS(CHECKSUM(NEWID())) AS Rand1,
        ABS(CHECKSUM(NEWID())) AS Rand2,
        ABS(CHECKSUM(NEWID())) AS Rand3,
        ABS(CHECKSUM(NEWID())) AS Rand4
    FROM Numbers
),
StudentData AS
(
    SELECT
        RowNo,
        Gender,

        CASE 
            WHEN Gender = 1 THEN
                CASE Rand1 % 15
                    WHEN 0 THEN 'Muhammad Ibrahim'
                    WHEN 1 THEN 'Ahmed Raza'
                    WHEN 2 THEN 'Muneeb Khan'
                    WHEN 3 THEN 'Haris Ahmed'
                    WHEN 4 THEN 'Rayan Ali'
                    WHEN 5 THEN 'Sameer Khan'
                    WHEN 6 THEN 'Arham Siddiqui'
                    WHEN 7 THEN 'Waleed Ahmed'
                    WHEN 8 THEN 'Zeeshan Ali'
                    WHEN 9 THEN 'Shahzaib Khan'
                    WHEN 10 THEN 'Umair Ahmed'
                    WHEN 11 THEN 'Noman Ali'
                    WHEN 12 THEN 'Rafay Khan'
                    WHEN 13 THEN 'Adeel Ahmed'
                    ELSE 'Mubashir Ali'
                END
            ELSE
                CASE Rand1 % 15
                    WHEN 0 THEN 'Alina Khan'
                    WHEN 1 THEN 'Minal Ahmed'
                    WHEN 2 THEN 'Areeba Ali'
                    WHEN 3 THEN 'Saba Siddiqui'
                    WHEN 4 THEN 'Mishal Khan'
                    WHEN 5 THEN 'Sehrish Ahmed'
                    WHEN 6 THEN 'Noor Fatima'
                    WHEN 7 THEN 'Anaya Khan'
                    WHEN 8 THEN 'Sidra Ahmed'
                    WHEN 9 THEN 'Aleena Ali'
                    WHEN 10 THEN 'Haniya Khan'
                    WHEN 11 THEN 'Maira Ahmed'
                    WHEN 12 THEN 'Rida Ali'
                    WHEN 13 THEN 'Sania Khan'
                    ELSE 'Zoya Ahmed'
                END
        END AS FullName,

        Rand2,
        Rand3,
        Rand4
    FROM RandomStudents
)
INSERT INTO Student
(
    FullName,
    Email,
    Phone,
    Gender,
    DateOfBirth,
    Address,
    AdmissionDate,
    IsActive
)
SELECT
    FullName,

    LOWER(
        REPLACE(FullName, ' ', '.')
        + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS VARCHAR(10))
        + '@gmail.com'
    ) AS Email,

    '03'
    + RIGHT('00' + CAST(ABS(Rand2) % 100 AS VARCHAR(2)), 2)
    + RIGHT('0000000' + CAST(ABS(Rand3) % 10000000 AS VARCHAR(7)), 7)
    AS Phone,

    Gender,

    DATEADD
    (
        DAY,
        -(6200 + (ABS(Rand3) % 6500)),
        GETDATE()
    ) AS DateOfBirth,

    CASE ABS(Rand4) % 12
        WHEN 0 THEN 'North Karachi, Karachi'
        WHEN 1 THEN 'Gulshan-e-Iqbal, Karachi'
        WHEN 2 THEN 'Nazimabad, Karachi'
        WHEN 3 THEN 'Federal B Area, Karachi'
        WHEN 4 THEN 'New Karachi, Karachi'
        WHEN 5 THEN 'Gulistan-e-Johar, Karachi'
        WHEN 6 THEN 'Buffer Zone, Karachi'
        WHEN 7 THEN 'Malir, Karachi'
        WHEN 8 THEN 'Korangi, Karachi'
        WHEN 9 THEN 'PECHS, Karachi'
        WHEN 10 THEN 'Shah Faisal Colony, Karachi'
        ELSE 'Scheme 33, Karachi'
    END AS Address,

    DATEADD
    (
        DAY,
        -(ABS(Rand2) % 730),
        GETDATE()
    ) AS AdmissionDate,

    CASE
        WHEN ABS(Rand4) % 10 = 0 THEN 0
        ELSE 1
    END AS IsActive

FROM StudentData;


SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    ---------------------------------------------------------
    -- 1. Backup current Student records with sequential IDs
    ---------------------------------------------------------
    IF OBJECT_ID('tempdb..#StudentBackup') IS NOT NULL
        DROP TABLE #StudentBackup;

    SELECT
        ROW_NUMBER() OVER (ORDER BY Id) AS NewId,
        FullName,
        Email,
        Phone,
        Gender,
        DateOfBirth,
        Address,
        AdmissionDate,
        IsActive
    INTO #StudentBackup
    FROM Student;


    ---------------------------------------------------------
    -- 2. Delete existing records
    ---------------------------------------------------------
    DELETE FROM Student;


    ---------------------------------------------------------
    -- 3. Reset identity
    ---------------------------------------------------------
    DBCC CHECKIDENT ('Student', RESEED, 0);


    ---------------------------------------------------------
    -- 4. Insert records back as 1,2,3,4...
    ---------------------------------------------------------
    SET IDENTITY_INSERT Student ON;

    INSERT INTO Student
    (
        Id,
        FullName,
        Email,
        Phone,
        Gender,
        DateOfBirth,
        Address,
        AdmissionDate,
        IsActive
    )
    SELECT
        NewId,
        FullName,
        Email,
        Phone,
        Gender,
        DateOfBirth,
        Address,
        AdmissionDate,
        IsActive
    FROM #StudentBackup
    ORDER BY NewId;

    SET IDENTITY_INSERT Student OFF;


    ---------------------------------------------------------
    -- 5. Make next ID continue after current maximum
    ---------------------------------------------------------
    DECLARE @MaxId INT;

    SELECT @MaxId = ISNULL(MAX(Id), 0)
    FROM Student;

    DBCC CHECKIDENT ('Student', RESEED, @MaxId);


    COMMIT TRANSACTION;


    ---------------------------------------------------------
    -- 6. Verify
    ---------------------------------------------------------
    SELECT *
    FROM Student
    ORDER BY Id;

    SELECT
        COUNT(*) AS TotalStudents,
        MIN(Id) AS FirstId,
        MAX(Id) AS LastId
    FROM Student;

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    -- Make sure IDENTITY_INSERT doesn't remain enabled
    BEGIN TRY
        SET IDENTITY_INSERT Student OFF;
    END TRY
    BEGIN CATCH
    END CATCH;

    THROW;

END CATCH;