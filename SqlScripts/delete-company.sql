BEGIN TRAN

-- list user 
SELECT * FROM AspNetUsers where Email = '';

-- ensure is Company
SELECT * FROM AspNetUserClaims where UserId = '';

-- list all the active bookings of company and NOTIFY CUSTOMERS
SELECT * FROM Bookings as b
JOIN Visits as  v on b.Id = v.BookingId
JOIN Events as e on e.Id  = v.EventId
WHERE e.UserId = '' and v.StartDate> 'TODAY-DATE';

-- run script to delete/move data to historical tables (separate sql file).  APPLY PROPER FILTERS.

-- DELETE Company
DELETE FROM AspNetUsers where Id = '' and Email ='';

COMMIT TRAN;