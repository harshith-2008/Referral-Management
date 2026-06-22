CREATE TABLE GlobalNetwork (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NetworkName     VARCHAR(150) NOT NULL
);

CREATE TABLE Hospital (
    HospitalId          INT IDENTITY(1,1) PRIMARY KEY,	
    HospitalName        VARCHAR(150) NOT NULL,
    HeadOfficeAddress   VARCHAR(255) NOT NULL,
    GlobalNetworkId     INT NULL,
    Latitude            DECIMAL(9,6) NULL,
    Longitude           DECIMAL(9,6) NULL,

    CONSTRAINT FK_Hospital_GlobalNetwork
        FOREIGN KEY (GlobalNetworkId)
        REFERENCES GlobalNetwork(Id)
);

CREATE TABLE Facility (
    FacilityId      INT IDENTITY(1,1) PRIMARY KEY,
    HospitalId      INT NOT NULL,
    FacilityName    VARCHAR(150) NOT NULL,
    City            VARCHAR(100) NULL,
    Address         VARCHAR(255) NULL,
    State           VARCHAR(100) NULL,
    PhoneNumber     VARCHAR(20) NULL,
    Latitude        DECIMAL(9,6) NULL,
    Longitude       DECIMAL(9,6) NULL,
    Status          BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Facility_Hospital
        FOREIGN KEY (HospitalId)
        REFERENCES Hospital(HospitalId)
);

CREATE TABLE Role (
    RoleId      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    VARCHAR(50) NOT NULL
);

CREATE TABLE ShiftBlock (
    ShiftBlockId    INT IDENTITY(1,1) PRIMARY KEY,
    StartTime       TIME(0) NOT NULL,
    EndTime         TIME(0) NOT NULL
);

CREATE TABLE Specialty (
    SpecialtyId     INT IDENTITY(1,1) PRIMARY KEY,
    SpecialtyName   VARCHAR(100) NOT NULL,
    Code            VARCHAR(20) NOT NULL
);

CREATE TABLE UrgencyLevel (
    UrgencyLevelId      INT IDENTITY(1,1) PRIMARY KEY,
    LevelName           VARCHAR(50) NOT NULL
);

CREATE TABLE ReferralStatus (
    ReferralStatusId    INT IDENTITY(1,1) PRIMARY KEY,
    StatusName          VARCHAR(50) NOT NULL
);

CREATE TABLE AppointmentStatus (
    AppointmentStatusId     INT IDENTITY(1,1) PRIMARY KEY,
    StatusName              VARCHAR(50) NOT NULL
);

CREATE TABLE [User] (
    UserId          INT IDENTITY(1,1) PRIMARY KEY,
    RoleId          INT NOT NULL,
    FacilityId      INT NOT NULL,
    FirstName       VARCHAR(100) NOT NULL,
    LastName        VARCHAR(100) NOT NULL,
    Email           VARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber     VARCHAR(20) NOT NULL,
    PasswordHash    VARCHAR(255) NOT NULL,
    Status          BIT NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    ValidTo   DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo),

    CONSTRAINT FK_User_Role
        FOREIGN KEY (RoleId)
        REFERENCES Role(RoleId),

    CONSTRAINT FK_User_Facility
        FOREIGN KEY (FacilityId)
        REFERENCES Facility(FacilityId)
)
WITH
(
    SYSTEM_VERSIONING = ON
    (
        HISTORY_TABLE = dbo.UserHistory
    )
);
CREATE TABLE Patient (
    PatientId               INT IDENTITY(1,1) PRIMARY KEY,
    Mrn                     VARCHAR(50) NOT NULL UNIQUE,
    UserId                  INT NOT NULL,
    PrimaryFacilityId       INT NOT NULL,
    Dob                     DATE NOT NULL,
    Gender                  CHAR(1) NOT NULL
                            CHECK (Gender IN ('M','F','O')),
    InsuranceProviderName   VARCHAR(150) NULL,
    InsuranceStatus         VARCHAR(50) NULL,
    PatientAddress          VARCHAR(255) NOT NULL,
    Status                  BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Patient_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_Patient_Facility
        FOREIGN KEY (PrimaryFacilityId)
        REFERENCES Facility(FacilityId)
);

CREATE TABLE Admin (
    AdminId     INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT NOT NULL,

    CONSTRAINT FK_Admin_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId)
);

CREATE TABLE ReferralCoordinator (
    ReferralCoordinatorId  INT IDENTITY(1,1) PRIMARY KEY,
    UserId                 INT NOT NULL,
    FacilityId             INT NOT NULL,

    CONSTRAINT FK_ReferralCoordinator_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_ReferralCoordinator_Facility
        FOREIGN KEY (FacilityId)
        REFERENCES Facility(FacilityId)
);

CREATE TABLE Specialist (
    SpecialistId    INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    FacilityId      INT NOT NULL,
    Status          BIT NOT NULL DEFAULT 1,
    ShiftBlockId    INT NULL,

    CONSTRAINT FK_Specialist_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_Specialist_Facility
        FOREIGN KEY (FacilityId)
        REFERENCES Facility(FacilityId),

    CONSTRAINT FK_Specialist_ShiftBlock
        FOREIGN KEY (ShiftBlockId)
        REFERENCES ShiftBlock(ShiftBlockId)
);

CREATE TABLE SpecialistSpecialities (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    SpecialtyId     INT NOT NULL,
    SpecialistId    INT NOT NULL,
    IsPrimary       BIT NOT NULL DEFAULT 0,

    CONSTRAINT UQ_SpecialistSpecialty
        UNIQUE (SpecialistId, SpecialtyId),

    CONSTRAINT FK_SpecialistSpecialities_Specialty
        FOREIGN KEY (SpecialtyId)
        REFERENCES Specialty(SpecialtyId),

    CONSTRAINT FK_SpecialistSpecialities_Specialist
        FOREIGN KEY (SpecialistId)
        REFERENCES Specialist(SpecialistId)
);

CREATE TABLE Referral (
    ReferralId              INT IDENTITY(1,1) PRIMARY KEY,
    PatientId               INT NOT NULL,
    OriginFacilityId        INT NOT NULL,
    DestinationFacilityId   INT NULL,
    CreatedByCoordinatorId  INT NULL,
    FromSpecialistId        INT NOT NULL,
    SpecialtyRequestId      INT NOT NULL,
    UrgencyLevelId          INT NOT NULL,
    ReferralStatusId        INT NOT NULL,
    ReferralReason          VARCHAR(500) NOT NULL,
    CreatedAt               DATETIME2(7) NULL,
    UpdatedAt               DATETIME2(7) NULL,
    DiagnosisCode           VARCHAR(20) NULL,

    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    ValidTo   DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo),

    CONSTRAINT FK_Referral_Patient
        FOREIGN KEY (PatientId)
        REFERENCES Patient(PatientId),

    CONSTRAINT FK_Referral_OriginFacility
        FOREIGN KEY (OriginFacilityId)
        REFERENCES Facility(FacilityId),

    CONSTRAINT FK_Referral_DestinationFacility
        FOREIGN KEY (DestinationFacilityId)
        REFERENCES Facility(FacilityId),

    CONSTRAINT FK_Referral_Coordinator
        FOREIGN KEY (CreatedByCoordinatorId)
        REFERENCES ReferralCoordinator(ReferralCoordinatorId),

    CONSTRAINT FK_Referral_FromSpecialist
        FOREIGN KEY (FromSpecialistId)
        REFERENCES Specialist(SpecialistId),

    CONSTRAINT FK_Referral_Specialty
        FOREIGN KEY (SpecialtyRequestId)
        REFERENCES Specialty(SpecialtyId),

    CONSTRAINT FK_Referral_UrgencyLevel
        FOREIGN KEY (UrgencyLevelId)
        REFERENCES UrgencyLevel(UrgencyLevelId),

    CONSTRAINT FK_Referral_ReferralStatus
        FOREIGN KEY (ReferralStatusId)
        REFERENCES ReferralStatus(ReferralStatusId)
)
WITH
(
    SYSTEM_VERSIONING = ON
    (
        HISTORY_TABLE = dbo.ReferralHistory
    )
);

CREATE TABLE ReferralAssignment (
    ReferralAssignmentId    INT IDENTITY(1,1) PRIMARY KEY,
    ReferralId              INT NOT NULL,
    AssignedBy              INT NOT NULL,
    FromSpecialistId        INT NULL,
    ToSpecialistId          INT NOT NULL,
    Reason                  VARCHAR(500) NULL,
    ReassignedAt            DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_ReferralAssignment_Referral
        FOREIGN KEY (ReferralId)
        REFERENCES Referral(ReferralId),

    CONSTRAINT FK_ReferralAssignment_AssignedBy
        FOREIGN KEY (AssignedBy)
        REFERENCES ReferralCoordinator(ReferralCoordinatorId),

    CONSTRAINT FK_ReferralAssignment_FromSpecialist
        FOREIGN KEY (FromSpecialistId)
        REFERENCES Specialist(SpecialistId),

    CONSTRAINT FK_ReferralAssignment_ToSpecialist
        FOREIGN KEY (ToSpecialistId)
        REFERENCES Specialist(SpecialistId)
);

CREATE TABLE Appointment (
    AppointmentId           INT IDENTITY(1,1) PRIMARY KEY,
    ReferralId              INT NOT NULL,
    SpecialistId            INT NOT NULL,
    PatientId               INT NOT NULL,
    AppointmentDate         DATE NOT NULL,
    AppointmentTime         TIME(0) NOT NULL,
    AppointmentStatusId     INT NOT NULL,

    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    ValidTo   DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo),

    CONSTRAINT FK_Appointment_Referral
        FOREIGN KEY (ReferralId)
        REFERENCES Referral(ReferralId),

    CONSTRAINT FK_Appointment_Specialist
        FOREIGN KEY (SpecialistId)
        REFERENCES Specialist(SpecialistId),

    CONSTRAINT FK_Appointment_Patient
        FOREIGN KEY (PatientId)
        REFERENCES Patient(PatientId),

    CONSTRAINT FK_Appointment_AppointmentStatus
        FOREIGN KEY (AppointmentStatusId)
        REFERENCES AppointmentStatus(AppointmentStatusId)
)
WITH
(
    SYSTEM_VERSIONING = ON
    (
        HISTORY_TABLE = dbo.AppointmentHistory
    )
);

CREATE TABLE AuditLog (
    AuditLogId  INT IDENTITY(1,1) PRIMARY KEY,
    ActionUser  INT NOT NULL,
    ReferralId  INT NULL,
    EntityType  VARCHAR(100) NOT NULL,
    Action      VARCHAR(100) NOT NULL,
    OldValues   NVARCHAR(MAX) NULL,
    NewValues   NVARCHAR(MAX) NULL,
    CreatedAt   DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_AuditLog_User
        FOREIGN KEY (ActionUser)
        REFERENCES [User](UserId),

    CONSTRAINT FK_AuditLog_Referral
        FOREIGN KEY (ReferralId)
        REFERENCES Referral(ReferralId)
);