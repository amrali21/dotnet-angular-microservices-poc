-- kpis
-- Values computed from "seed data - invoice.sql" / "seed data - customer.sql":
--   Total Billed = sum of all invoice amounts
--   Collected    = sum of amounts where status = 'paid'
--   Outstanding  = sum of amounts where status = 'pending'
--   Customers    = row count of seeded customers
INSERT INTO [ledgerly-dashbaord].[dbo].[kpis] (ID, kpiname, kpivalue) VALUES
(1, 'Total Billed', 218954),
(2, 'Collected', 100125),
(3, 'Outstanding', 118829),
(4, 'Customers', 9);

-- Revenue
-- One row per month for each year present in the seeded invoices (2022, 2023),
-- value = sum of 'paid' invoice amounts whose date falls in that month/year.
INSERT INTO [ledgerly-dashbaord].[dbo].[revenue] (month, year, revenue) VALUES
('Jan', 2022, 0),
('Feb', 2022, 0),
('Mar', 2022, 0),
('Apr', 2022, 0),
('May', 2022, 0),
('Jun', 2022, 1000),
('Jul', 2022, 0),
('Aug', 2022, 0),
('Sep', 2022, 0),
('Oct', 2022, 3040),
('Nov', 2022, 0),
('Dec', 2022, 0),
('Jan', 2023, 0),
('Feb', 2023, 0),
('Mar', 2023, 0),
('Apr', 2023, 0),
('May', 2023, 0),
('Jun', 2023, 51285),
('Jul', 2023, 0),
('Aug', 2023, 0),
('Sep', 2023, 44800),
('Oct', 2023, 0),
('Nov', 2023, 0),
('Dec', 2023, 0);
