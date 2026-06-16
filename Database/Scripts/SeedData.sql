--GLOBAL NETWORK
INSERT INTO GlobalNetwork (NetworkName) VALUES ( 'Global Network');

--HOSPITALS
INSERT INTO Hospital
(HospitalName, HeadOfficeAddress, GlobalNetworkId, Latitude, Longitude)
VALUES
('HCA Healthcare', 'Nashville, Tennessee, USA', 1, 36.1627, -86.7816);
INSERT INTO Hospital
(HospitalName, HeadOfficeAddress, GlobalNetworkId, Latitude, Longitude)
VALUES
('Ascension', 'St. Louis, Missouri, USA', 1, 38.6270, -90.1994);
INSERT INTO Hospital
(HospitalName, HeadOfficeAddress, GlobalNetworkId, Latitude, Longitude)
VALUES
('CommonSpirit Health', 'Chicago, Illinois, USA', 1, 41.8781, -87.6298);
INSERT INTO Hospital
(HospitalName, HeadOfficeAddress, GlobalNetworkId, Latitude, Longitude)
VALUES
('Cleveland Clinic', 'Cleveland, Ohio, USA', 1, 41.4993, -81.6944);
INSERT INTO Hospital
(HospitalName, HeadOfficeAddress, GlobalNetworkId, Latitude, Longitude)
VALUES
('Mayo Clinic', 'Rochester, Minnesota, USA', 1, 44.0121, -92.4802);

--FACILITIES
-- HCA Healthcare (HospitalId = 1)
INSERT INTO Facility VALUES
(1,'HCA TriStar Centennial Medical Center','Nashville','2300 Patterson St, Nashville, TN','Tennessee','6153421000',36.151900,-86.806600,1);
INSERT INTO Facility VALUES
(1,'HCA TriStar Skyline Medical Center','Nashville','3441 Dickerson Pike, Nashville, TN','Tennessee','6157692000',36.222500,-86.758700,1);
INSERT INTO Facility VALUES
(1,'HCA Houston Healthcare Medical Center','Houston','1313 Hermann Dr, Houston, TX','Texas','7137901234',29.710800,-95.388500,1);
INSERT INTO Facility VALUES
(1,'HCA Florida Brandon Hospital','Brandon','119 Oakfield Dr, Brandon, FL','Florida','8136815551',27.938100,-82.286500,1);
INSERT INTO Facility VALUES
(1,'HCA HealthONE Swedish Medical Center','Englewood','501 E Hampden Ave, Englewood, CO','Colorado','3037885000',39.653400,-104.981900,1);
-- Ascension (HospitalId = 2)
INSERT INTO Facility VALUES
(2,'Ascension Saint Thomas Hospital Midtown','Nashville','2000 Church St, Nashville, TN','Tennessee','6152845555',36.152800,-86.801000,1);
INSERT INTO Facility VALUES
(2,'Ascension St. Vincent Indianapolis','Indianapolis','2001 W 86th St, Indianapolis, IN','Indiana','3173382345',39.911700,-86.195500,1);
-- CommonSpirit Health (HospitalId = 3)
INSERT INTO Facility VALUES
(3,'St. Joseph Hospital Denver','Denver','1375 E 19th Ave, Denver, CO','Colorado','3038122000',39.746900,-104.971700,1);
INSERT INTO Facility VALUES
(3,'Baylor St. Lukes Medical Center','Houston','6720 Bertner Ave, Houston, TX','Texas','8323551000',29.707300,-95.398000,1);
-- Cleveland Clinic (HospitalId = 4)
INSERT INTO Facility VALUES
(4,'Cleveland Clinic Main Campus','Cleveland','9500 Euclid Ave, Cleveland, OH','Ohio','2164442200',41.503600,-81.620900,1);
INSERT INTO Facility VALUES
(4,'Cleveland Clinic Weston Hospital','Weston','2950 Cleveland Clinic Blvd, Weston, FL','Florida','9546595000',26.082700,-80.363600,1);
-- Mayo Clinic (HospitalId = 5)
INSERT INTO Facility VALUES
(5,'Mayo Clinic Rochester','Rochester','200 First St SW, Rochester, MN','Minnesota','5072842511',44.022100,-92.466000,1);
INSERT INTO Facility VALUES
(5,'Mayo Clinic Phoenix','Phoenix','5777 E Mayo Blvd, Phoenix, AZ','Arizona','4803422000',33.658400,-111.945600,1);

--ROLES
INSERT INTO Role (RoleName)
VALUES ('Admin');
INSERT INTO Role (RoleName)
VALUES ('ReferralCoordinator');
INSERT INTO Role (RoleName)
VALUES ('Patient');
INSERT INTO Role (RoleName)
VALUES ('Specialist');

--URGENCY LEVEL
INSERT INTO UrgencyLevel (LevelName)
VALUES
('Routine'),
('Urgent'),
('Emergency');

--SHIFT BLOCK
INSERT INTO ShiftBlock (StartTime, EndTime)
VALUES
('07:00', '15:00'),
('15:00', '23:00'),
('23:00', '07:00');

--Referral Status
INSERT INTO ReferralStatus (StatusName)
VALUES
('Requested'),
('Submitted'),
('Accepted'),
('Rejected'),
('Scheduled'),
('Completed'),
('Closed');

--Appointment status
INSERT INTO AppointmentStatus (StatusName)
VALUES
('Scheduled'),
('Cancelled'),
('Completed');

--Speciality
INSERT INTO Specialty (SpecialtyName, Code)
VALUES
('Cardiology', 'CARD'),
('Dermatology', 'DERM'),
('Endocrinology', 'ENDO'),
('Gastroenterology', 'GAST'),
('Neurology', 'NEUR'),
('Obstetrics & Gynecology', 'OBGY'),
('Oncology', 'ONCO'),
('Orthopedics', 'ORTH'),
('Pulmonology', 'PULM'),
('Urology', 'UROL');

