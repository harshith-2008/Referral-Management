-- Global Network
INSERT INTO GlobalNetwork (NetworkName)
VALUES ('Global Health Network');

-- Hospital
INSERT INTO Hospital (HospitalName, HeadOfficeAddress, GlobalNetworkId)
VALUES ('Apollo Hospitals','Hyderabad HO',1);

-- Facility
INSERT INTO Facility (HospitalId, FacilityName, City, Status)
VALUES
(1,'Apollo Main','Hyderabad',1),
(1,'Apollo Branch','Hyderabad',1);

-- Roles
INSERT INTO Role (RoleName)
VALUES ('Admin'), ('Coordinator'), ('Patient'), ('Specialist');

-- Shift Blocks
INSERT INTO ShiftBlock (StartTime, EndTime)
VALUES ('09:00','13:00'),('13:00','18:00');

-- Specialties
INSERT INTO Specialty (SpecialtyName, Code)
VALUES 
('Cardiology','CARD'),
('Neurology','NEUR');

-- Urgency
INSERT INTO UrgencyLevel (LevelName)
VALUES ('Low'),('Medium'),('High');

-- Referral Status
INSERT INTO ReferralStatus (StatusName)
VALUES
('Pending'),
('Requested'),
('Accepted'),
('Scheduled'),
('Completed'),
('Cancelled');

-- Appointment Status
INSERT INTO AppointmentStatus (StatusName)
VALUES
('Scheduled'),
('Cancelled'),
('Completed');
