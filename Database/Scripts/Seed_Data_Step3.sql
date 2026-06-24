INSERT INTO Referral
(
    PatientId,
    OriginFacilityId,
    DestinationFacilityId,
    CreatedByCoordinatorId,
    FromSpecialistId,
    SpecialtyRequestId,
    UrgencyLevelId,
    ReferralStatusId,
    ReferralReason,
    CreatedAt,
    UpdatedAt
)
SELECT TOP 5
    p.PatientId,
    1,
    2,
    rc.ReferralCoordinatorId,
    s.SpecialistId,
    1,
    1,
    rs.ReferralStatusId,
    'Generated Referral',
    GETUTCDATE(),
    GETUTCDATE()
FROM Patient p
JOIN ReferralCoordinator rc ON 1=1
JOIN Specialist s ON 1=1
JOIN ReferralStatus rs ON rs.StatusName = 'Pending';

INSERT INTO Appointment
(
    ReferralId,
    SpecialistId,
    PatientId,
    AppointmentDate,
    AppointmentTime,
    AppointmentStatusId
)
SELECT
    r.ReferralId,
    s.SpecialistId,
    r.PatientId,
    CAST(GETUTCDATE() AS DATE),
    '10:00',
    aps.AppointmentStatusId
FROM Referral r
JOIN Specialist s ON 1=1
JOIN AppointmentStatus aps ON aps.StatusName='Scheduled';

INSERT INTO AuditLog
(ActionUser, ReferralId, EntityType, Action)
SELECT
    u.UserId,
    r.ReferralId,
    'Referral',
    'Created'
FROM [User] u
JOIN Referral r ON 1=1;