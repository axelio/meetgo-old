
-- TRANSACTION BEGIN
begin tran;

-- INSERT BOOKINGS

SET IDENTITY_INSERT HistoricalBookings ON;
GO
INSERT INTO HistoricalBookings
(Id, CreatedDate, IsConfirmed, UserId, VisitId)
	SELECT b.Id, b.CreatedDate, b.IsConfirmed, b.UserId, b.VisitId FROM Bookings as b
	INNER JOIN Visits as v on v.Id = b.VisitId
	WHERE v.StartDate < 'DATE'
GO
SET IDENTITY_INSERT HistoricalBookings OFF;

select * from HistoricalVisits;

-- INSERT VISITS
GO
SET IDENTITY_INSERT HistoricalVisits ON;
GO
INSERT INTO HistoricalVisits
(Id, CreatedDate, StartDate, Price, MaxPersons, EventId, TimeOfDay, UserId)
	SELECT v.Id, v.CreatedDate, v.StartDate, v.Price, v.MaxPersons, v.EventId, v.TimeOfDay, e.UserId FROM Visits as v
	JOIN Events as e ON e.Id = v.EventId
	WHERE v.StartDate < 'DATE'
GO
SET IDENTITY_INSERT HistoricalVisits OFF;
GO

-- DELETE BOOKINGS

DELETE b FROM Bookings b
	INNER JOIN Visits as v on v.Id = b.VisitId
	WHERE v.StartDate < 'DATE'

-- DELETE VISITS
DELETE FROM Visits where StartDate < 'DATE';
GO

-- CHECK

SELECT * FROM HistoricalVisits;

SELECT * FROM HistoricalBookings;

SELECT * FROM Visits where StartDate < 'DATE';

SELECT * FROM Bookings as b
	INNER JOIN Visits as v on v.Id = b.VisitId
	WHERE v.StartDate < 'DATE'

commit tran;
--TRANSACTION END