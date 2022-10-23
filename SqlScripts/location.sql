select * from Addresses;

--lat long srid
UPDATE Addresses SET Location = geography::Point(47.65100, -122.34900, 4326) 