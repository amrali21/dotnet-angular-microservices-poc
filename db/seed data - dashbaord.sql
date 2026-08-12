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
-- One row per month for 2026, the only year present in the seeded invoices,
-- value = sum of 'paid' invoice amounts whose date falls in that month/year.
INSERT INTO [ledgerly-dashbaord].[dbo].[revenue] (month, year, revenue) VALUES
('Jan', 2026, 8545),
('Feb', 2026, 0),
('Mar', 2026, 8945),
('Apr', 2026, 0),
('May', 2026, 1000),
('Jun', 2026, 0),
('Jul', 2026, 3040),
('Aug', 2026, 44800),
('Sep', 2026, 0),
('Oct', 2026, 0),
('Nov', 2026, 0),
('Dec', 2026, 33795);
