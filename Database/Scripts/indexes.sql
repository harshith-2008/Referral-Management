CREATE INDEX IX_Referral_ReferralStatusId
ON Referral(ReferralStatusId);


CREATE INDEX IX_Referral_CreatedAt
ON Referral(CreatedAt);


CREATE INDEX IX_Referral_PatientId
ON Referral(PatientId);


CREATE INDEX IX_Referral_OriginFacilityId
ON Referral(OriginFacilityId);


CREATE INDEX IX_Referral_SpecialtyRequestId
ON Referral(SpecialtyRequestId);
CREATE INDEX IX_Referral_Status_Created
ON Referral(ReferralStatusId, CreatedAt);

CREATE INDEX IX_Appointment_Specialist_Date
ON Appointment(SpecialistId, AppointmentDate);


CREATE INDEX IX_Appointment_ReferralId
ON Appointment(ReferralId);


CREATE INDEX IX_Appointment_PatientId
ON Appointment(PatientId);

CREATE INDEX IX_Appointment_Referral_Date
ON Appointment(ReferralId, AppointmentDate);

CREATE INDEX IX_ReferralAssignment_ToSpecialistId
ON ReferralAssignment(ToSpecialistId);

CREATE INDEX IX_Specialist_UserId
ON Specialist(UserId);

CREATE INDEX IX_Patient_UserId
ON Patient(UserId);

CREATE INDEX IX_ReferralStatus_StatusName
ON ReferralStatus(StatusName);


CREATE INDEX IX_AppointmentStatus_StatusName
ON AppointmentStatus(StatusName);
