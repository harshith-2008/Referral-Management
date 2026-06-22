/* ============================================================
   Coordinator Dashboard - Requested Referrals
   ============================================================ */
GO

CREATE OR ALTER VIEW dbo.vw_Coordinator_RequestedReferrals
AS
SELECT
    r.ReferralId,
    r.DestinationFacilityId,

    -- Patient
    p.PatientId,
    u.FirstName + ' ' + u.LastName AS PatientName,

    -- Facilities
    ofac.FacilityName AS OriginFacility,
    dfac.FacilityName AS DestinationFacility,

    -- Referral Info
    rs.StatusName AS Status,
    ul.LevelName AS Urgency,
    sp.SpecialtyName AS Specialty,
    r.DiagnosisCode,
    r.CreatedAt
FROM Referral r
INNER JOIN Patient p
    ON r.PatientId = p.PatientId
INNER JOIN [User] u
    ON p.UserId = u.UserId
INNER JOIN Facility ofac
    ON r.OriginFacilityId = ofac.FacilityId
INNER JOIN Facility dfac
    ON r.DestinationFacilityId = dfac.FacilityId
INNER JOIN ReferralStatus rs
    ON r.ReferralStatusId = rs.ReferralStatusId
INNER JOIN UrgencyLevel ul
    ON r.UrgencyLevelId = ul.UrgencyLevelId
INNER JOIN Specialty sp
    ON r.SpecialtyRequestId = sp.SpecialtyId
WHERE rs.StatusName = 'Requested';
GO


/* ============================================================
   Specialist Dashboard - Assigned Patients
   ============================================================ */
GO

CREATE OR ALTER VIEW dbo.vw_Specialist_AssignedPatients
AS
SELECT
    ra.ToSpecialistId AS SpecialistId,
    r.ReferralId,

    -- Patient
    p.PatientId,
    u.FirstName + ' ' + u.LastName AS PatientName,
    p.Mrn,
    p.Gender,
    p.Dob,

    -- Referral
    sp.SpecialtyName AS Specialty,
    ul.LevelName AS Urgency,
    r.DiagnosisCode,
    r.CreatedAt
FROM ReferralAssignment ra
INNER JOIN Referral r
    ON ra.ReferralId = r.ReferralId
INNER JOIN ReferralStatus rs
    ON r.ReferralStatusId = rs.ReferralStatusId
INNER JOIN Patient p
    ON r.PatientId = p.PatientId
INNER JOIN [User] u
    ON p.UserId = u.UserId
INNER JOIN Specialty sp
    ON r.SpecialtyRequestId = sp.SpecialtyId
INNER JOIN UrgencyLevel ul
    ON r.UrgencyLevelId = ul.UrgencyLevelId
WHERE rs.StatusName = 'Scheduled';
GO


/* ============================================================
   Appointment Details
   ============================================================ */
GO

CREATE OR ALTER VIEW dbo.vw_AppointmentDetails
AS
SELECT
    a.AppointmentId,
    a.ReferralId,
    a.PatientId,
    a.SpecialistId,

    a.AppointmentDate,
    a.AppointmentTime,

    aps.StatusName AS AppointmentStatus,

    -- Patient
    pu.FirstName + ' ' + pu.LastName AS PatientName,
    p.Mrn,

    -- Specialist
    su.FirstName + ' ' + su.LastName AS SpecialistName,

    -- Referral
    r.ReferralReason
FROM Appointment a
INNER JOIN AppointmentStatus aps
    ON a.AppointmentStatusId = aps.AppointmentStatusId
INNER JOIN Patient p
    ON a.PatientId = p.PatientId
INNER JOIN [User] pu
    ON p.UserId = pu.UserId
INNER JOIN Specialist s
    ON a.SpecialistId = s.SpecialistId
INNER JOIN [User] su
    ON s.UserId = su.UserId
INNER JOIN Referral r
    ON a.ReferralId = r.ReferralId;
GO